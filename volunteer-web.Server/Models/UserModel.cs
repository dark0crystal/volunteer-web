namespace volunteer_web.Server.Models
{
    public class UserModel
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string familyName { get; set; }
        public string email { get; set; }
        public string password { get; set; }

    }

    public enum UserType { 
        Basic,
        Admin
    }
}


