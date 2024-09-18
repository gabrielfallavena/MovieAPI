namespace API.Models
{
    public class ReviewDTO
    {
        public float Rating { get; set; }
        public string? Comment { get; set; }
        public string? MovieTitle { get; set; }
        public DateTime Date { get; set; }
        public string? UserName { get; set; }
    }
}
