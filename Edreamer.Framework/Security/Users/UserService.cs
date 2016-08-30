using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Edreamer.Framework.Domain;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Injection;
using Edreamer.Framework.Logging;
using Edreamer.Framework.Security.Encryption;
using Edreamer.Framework.Settings;
using Edreamer.Framework.Data.Extensions;

namespace Edreamer.Framework.Security.Users
{
    public class UserService : IUserService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly ISettingsService _settingsService;
        private readonly IEnumerable<IUserEventHandler> _userEventHandlers;
        private readonly IFrameworkDataContext _dataContext;

        public UserService(ISettingsService settingsService, IEncryptionService encryptionService, IEnumerable<IUserEventHandler> userEventHandlers, IFrameworkDataContext dataContext)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            Throw.IfArgumentNull(encryptionService, "encryptionService");
            Throw.IfArgumentNull(dataContext, "dataContext");
            _encryptionService = encryptionService;
            _settingsService = settingsService;
            _userEventHandlers = CollectionHelpers.EmptyIfNull(userEventHandlers);
            _dataContext = dataContext;
        }

        public ILogger Logger { get; set; }

        public User AddUser(User user, string password)
        {
            // ToDo [01231232]: Validate user
            Throw.IfArgumentNull(user, "user");
            Throw.IfArgumentNull(password, "password");

            Logger.Information("CreateUser {0} {1}", user.Username, user.Email);

            var userContext = new UserContext { User = user, Cancel = false };
            _userEventHandlers.ForEach(ueh => ueh.Creating(userContext));

            if (userContext.Cancel)
            {
                return null;
            }

            var userEntity = Injector.PlaneInject(new Domain.User(), user);
            userEntity.UserData = SerializationHelpers.Serialize(user.UserData);
            userEntity.HashAlgorithm = _settingsService.GetMembershipSettings().PasswordHashAlgorithm;
            SetPassword(userEntity, password);

            _dataContext.Users.Add(userEntity);
            _dataContext.SaveChanges();

            user.Id = userEntity.Id;
            userContext.User = user; // In case it's been replaced
            foreach (var userEventHandler in _userEventHandlers)
            {
                userEventHandler.Created(userContext);
            }
            return user;
        }

        public void UpdateUser(User user)
        {
            // ToDo [01231240]: Validate user
            Throw.IfArgumentNull(user, "user");
            var userEntity = Injector.PlaneInject(new Domain.User { UserData = SerializationHelpers.Serialize(user.UserData) }, user);
            _dataContext.Users.Update(userEntity, u => new { u.Username, u.Email, u.Approved, u.EmailConfirmed, u.Disabled, u.UserData });
            _dataContext.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            _dataContext.Users.Remove(null, id);
            _dataContext.SaveChanges();
        }

        public void SetPassword(int id, string password)
        {
            // ToDo [01231245]: Validate password
            // User's HashAlgorithm value may be required so fetching user is inevitable
            var userEntity = _dataContext.Users.Find(id);
            Throw.IfNull(userEntity)
                .AnArgumentException("There's no user with id {0}.".FormatWith(id), "id");
            SetPassword(userEntity, password);
            _dataContext.Users.Update(userEntity, u => new { u.Password, u.PasswordFormat, u.PasswordSalt });
            _dataContext.SaveChanges();
            var user = Injector.PlaneInject(new User(), userEntity);
            foreach (var userEventHandler in _userEventHandlers)
            {
                userEventHandler.ChangedPassword(user);
            }
        }

        public User ValidateUser(string usernameOrEmail, string password, bool validateStatus = true)
        {
            Throw.IfArgumentNullOrEmpty(usernameOrEmail, "usernameOrEmail");
            Throw.IfArgumentNullOrEmpty(password, "password");

            var lowerName = usernameOrEmail.ToLower();
            var userEntity = _dataContext.Users.SingleOrDefault(u =>
                u.Username.ToLower() == lowerName || u.Email.ToLower() == lowerName);

            if (userEntity == null || !ValidatePassword(userEntity, password))
            {
                return null;
            }

            if (validateStatus && (!userEntity.Approved || !userEntity.EmailConfirmed || userEntity.Disabled))
            {
                return null;
            }

            var user = Injector.PlaneInject(new User(), userEntity);
            user.UserData = SerializationHelpers.Deserialize(userEntity.UserData);
            return user;
        }

        public User GetUser(int id)
        {
            var userEntity = _dataContext.Users.Find(id);
            if (userEntity == null) return null;
            var user = Injector.PlaneInject(new User(), userEntity);
            user.UserData = SerializationHelpers.Deserialize(userEntity.UserData);
            return user;
        }

        public User GetUser(string usernameOrEmail)
        {
            Throw.IfArgumentNullOrEmpty(usernameOrEmail, "usernameOrEmail");
            var lowerName = usernameOrEmail.ToLower();
            var userEntity = _dataContext.Users.SingleOrDefault(u =>
                u.Username.ToLower() == lowerName || u.Email.ToLower() == lowerName);
            if (userEntity == null) return null;
            var user = Injector.PlaneInject(new User(), userEntity);
            user.UserData = SerializationHelpers.Deserialize(userEntity.UserData);
            return user;
        }

        public void SetRolesOfUser(int id, IEnumerable<Role> roles)
        {
            roles = CollectionHelpers.EmptyIfNull(roles);
            var userEntity = _dataContext.Users.WithTracking().Include(u => u.Roles).Find(id);
            Throw.IfNull(userEntity)
                .AnArgumentException("There's no user with id {0}.".FormatWith(id), "id");
            var rolesToSet = roles.Select(r => Injector.PlaneInject(new Domain.Role(), r));
            userEntity.Roles.SetCollection(rolesToSet,
                                           r => { _dataContext.Roles.Attach(r); return r; }, // Attaching stub roles prevents putting them in added state
                                           null,
                                           r => r.Id);
            _dataContext.SaveChanges();
        }

        public IEnumerable<Role> GetRolesOfUser(int id)
        {
            var userEntity = _dataContext.Users.Include(u => u.Roles).Find(id);
            return userEntity == null
                       ? Enumerable.Empty<Role>()
                       : userEntity.Roles.Select(r => Injector.PlaneInject(new Role(), r));
        }

        public bool VerifyUsernameUnicity(string username, int? id = null)
        {
            Throw.IfArgumentNullOrEmpty(username, "username");
            var lowerName = username.ToLower();
            return id == null
                       ? !_dataContext.Users.Any(u => u.Username.ToLower() == lowerName)
                       : !_dataContext.Users.Any(u => u.Id != id && u.Username.ToLower() == lowerName);
        }

        public bool VerifyEmailUnicity(string email, int? id = null)
        {
            Throw.IfArgumentNullOrEmpty(email, "email");
            var lowerEmail = email.ToLower();
            return id == null
                       ? !_dataContext.Users.Any(u => u.Email.ToLower() == lowerEmail)
                       : !_dataContext.Users.Any(u => u.Id != id && u.Email.ToLower() == lowerEmail);
        }

        #region Private Methods
        private void SetPassword(Domain.User userEntity, string password)
        {
            switch (_settingsService.GetMembershipSettings().PasswordFormat)
            {
                case PasswordFormat.Clear:
                    SetPasswordClear(userEntity, password);
                    break;
                case PasswordFormat.Hashed:
                    SetPasswordHashed(userEntity, password);
                    break;
                case PasswordFormat.Encrypted:
                    SetPasswordEncrypted(userEntity, password, _encryptionService);
                    break;
                default:
                    Throw.Now.A<ApplicationException>("Unexpected password format value");
                    break;
            }
        }

        private bool ValidatePassword(Domain.User userEntity, string password)
        {
            // Note: the password format stored with the record is used
            // otherwise changing the password format on the site would invalidate
            // all logins
            switch ((PasswordFormat)userEntity.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    return ValidatePasswordClear(userEntity, password);
                case PasswordFormat.Hashed:
                    return ValidatePasswordHashed(userEntity, password);
                case PasswordFormat.Encrypted:
                    return ValidatePasswordEncrypted(userEntity, password, _encryptionService);
                default:
                    Throw.Now.A<ApplicationException>("Unexpected password format value");
                    return false;
            }
        }

        private static void SetPasswordClear(Domain.User userEntity, string password)
        {
            userEntity.PasswordFormat = PasswordFormat.Clear;
            userEntity.Password = password;
            userEntity.PasswordSalt = null;
        }

        private static bool ValidatePasswordClear(Domain.User userEntity, string password)
        {
            return userEntity.Password == password;
        }

        private static void SetPasswordHashed(Domain.User userEntity, string password)
        {

            var saltBytes = new byte[0x10];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(saltBytes);
            }

            var passwordBytes = Encoding.Unicode.GetBytes(password);

            var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();

            byte[] hashBytes;
            using (var hashAlgorithm = HashAlgorithm.Create(userEntity.HashAlgorithm))
            {
                hashBytes = hashAlgorithm.ComputeHash(combinedBytes);
            }

            userEntity.PasswordFormat = PasswordFormat.Hashed;
            userEntity.Password = Convert.ToBase64String(hashBytes);
            userEntity.PasswordSalt = Convert.ToBase64String(saltBytes);
        }

        private static bool ValidatePasswordHashed(Domain.User userEntity, string password)
        {

            var saltBytes = Convert.FromBase64String(userEntity.PasswordSalt);

            var passwordBytes = Encoding.Unicode.GetBytes(password);

            var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();

            byte[] hashBytes;
            using (var hashAlgorithm = HashAlgorithm.Create(userEntity.HashAlgorithm))
            {
                hashBytes = hashAlgorithm.ComputeHash(combinedBytes);
            }

            return userEntity.Password == Convert.ToBase64String(hashBytes);
        }

        private static void SetPasswordEncrypted(Domain.User userEntity, string password, IEncryptionService encryptionService)
        {
            userEntity.PasswordFormat = PasswordFormat.Encrypted;
            userEntity.Password = Convert.ToBase64String(encryptionService.Encode(Encoding.UTF8.GetBytes(password)));
            userEntity.PasswordSalt = null;
        }

        private static bool ValidatePasswordEncrypted(Domain.User userEntity, string password, IEncryptionService encryptionService)
        {
            return String.Equals(password, Encoding.UTF8.GetString(encryptionService.Decode(Convert.FromBase64String(userEntity.Password))), StringComparison.Ordinal);
        }
        #endregion
    }
}
