namespace API.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Director { get; set; } 
        public string? Genre { get; set; } 
        public DateTime ReleaseDate { get; set; }
        public string? Synopsis { get; set; }
        public string? PosterUrl { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
