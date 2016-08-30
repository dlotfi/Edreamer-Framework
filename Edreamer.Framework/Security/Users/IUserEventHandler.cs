namespace Edreamer.Framework.Security.Users
{
    public interface IUserEventHandler
    {
        /// <summary>
        /// Called before a User is created
        /// </summary>
        void Creating(UserContext context);

        /// <summary>
        /// Called after a user has been created
        /// </summary>
        void Created(UserContext context);

        /// <summary>
        /// Called after a user has logged in
        /// </summary>
        void LoggedIn(User user);

        /// <summary>
        /// Called when a user explicitly logs out (as opposed to one whose session cookie simply expires)
        /// </summary>
        void LoggedOut(User user);

        /// <summary>
        /// Called when access is denied to a user
        /// </summary>
        void AccessDenied(User user);

        /// <summary>
        /// Called after a user has changed password
        /// </summary>
        void ChangedPassword(User user);
    }

    public class UserContext
    {
        public User User { get; set; }
        public bool Cancel { get; set; }
    }
}

