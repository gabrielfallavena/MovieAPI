namespace API.Models
{
    public class ReviewRequest
    {
        public string? MovieName { get; set; } // Nome do filme ao qual a review está associada
        public float Rating { get; set; }
        public string? Comment { get; set; }
    }
}
