namespace Edreamer.Framework.Security
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Approved { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Disabled { get; set; }
        public object UserData { get; set; }
    }
}
