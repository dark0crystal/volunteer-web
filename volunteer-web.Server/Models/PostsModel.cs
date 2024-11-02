namespace volunteer_web.Server.Models
{
    public class PostsModel
    {
        public int id { get; set; }
        public string postAdminEmail { get; set; }
        public string title { get; set; }
        public PostCategory category { get; set; }
        public string description { get; set; }
        public int numOfDays { get; set; }
        public string location { get; set; }
        public string orgName { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }



    }
}

public enum PostCategory
{
    Education,
    Environment,
    Health,
    Community,
    AnimalWelfare,
 
}
