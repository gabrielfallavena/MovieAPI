namespace API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public float Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string MovieTitle { get; set; } = string.Empty;
        public int MovieId { get; set; }
    }
}
