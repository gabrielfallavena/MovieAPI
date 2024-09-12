namespace API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;

        public List<Review> Reviews { get; set; } = new List<Review>();

        public User() { }
        public User(int id, string name, string email, string password, string nickName)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            NickName = nickName;
        }
    }
}
