namespace SnipperSnippetsAPI.Entities
{
    public class User
    {
        public int id { get; set; }
        public required string email { get; set; }
        public required string password { get; set; }
    }
}
