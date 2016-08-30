using System.Collections.Generic;

namespace Edreamer.Framework.Security.Users
{
    public interface IUserService
    {
        User AddUser(User user, string password);
        void UpdateUser(User user);
        void DeleteUser(int id);

        void SetPassword(int id, string password);
        User ValidateUser(string usernameOrEmail, string password, bool validateStatus = true);

        User GetUser(int id);
        User GetUser(string usernameOrEmail);

        void SetRolesOfUser(int id, IEnumerable<Role> roles);
        IEnumerable<Role> GetRolesOfUser(int id);

        bool VerifyUsernameUnicity(string username, int? id = null);
        bool VerifyEmailUnicity(string email, int? id = null);
    }
}