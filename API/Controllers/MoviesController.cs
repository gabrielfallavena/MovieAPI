using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly APIContext _context;
    private readonly HttpClient _httpClient;
    private readonly string _omdbApiKey = "21eaea2";

    public MoviesController(APIContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    // GET: api/Movies/{movieName}
    [HttpGet("{movieName}")]
    public async Task<IActionResult> GetMovieFromOmdb(string movieName)
    {
        var response = await _httpClient.GetAsync($"http://www.omdbapi.com/?t={movieName}&apikey={_omdbApiKey}");

        if (!response.IsSuccessStatusCode) return NotFound("Filme não encontrado na OMDb.");   

        var omdbMovie = await response.Content.ReadFromJsonAsync<OMDbMovieResponse>();

        if (omdbMovie == null || omdbMovie.Title == null) return NotFound("Filme não encontrado.");

        return Ok(omdbMovie);
    }

    [HttpGet("searchMovies")]
    public async Task<IActionResult> SearchMovies(string query)
    {
        if (string.IsNullOrEmpty(query)) return BadRequest("Search query cannot be empty.");

        query = query.ToLower();

        var movies = await _context.Movie
            .Where(m => m.Title.ToLower().Contains(query)).ToListAsync();

        return Ok(movies);
    }

    // GET: Obtem dados do filme
    [HttpGet("{movieTitle}/MovieData")]
    public async Task<IActionResult> GetMovieData(string movieTitle)
    {
        var movie = await _context.Movie.FirstOrDefaultAsync(m => m.Title == movieTitle);

        if (movie == null) return NotFound("Filme não está no banco de dados");

        return Ok(movie);
    }

    // Busca as reviews do filme passado como parametro
    [HttpGet("{movieTitle}/Reviews")]
    public async Task<IActionResult> GetReviewsForMovie(string movieTitle)
    {
        var reviews = await (from review in _context.Review
                             join user in _context.User
                             on review.UserId equals user.Id
                             where review.MovieTitle == movieTitle
                             select new ReviewDTO
                             {
                                 Rating = review.Rating,
                                 Comment = review.Comment,
                                 MovieTitle = review.MovieTitle,
                                 Date = review.Date,
                                 UserName = user.Name
                             }).ToListAsync();

        return Ok(reviews);
    }
}