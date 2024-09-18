namespace API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; } 
        public string? Email { get; set; }
        public string? NickName { get; set; }
        public string? Password { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<User> Friends { get; set; } = new List<User>();

        public User() { }
    }
}
