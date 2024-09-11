using API.Data;
using API.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly APIContext _context;
    private readonly HttpClient _httpClient;
    private readonly string _omdbApiKey = "21eaea2";

    public ReviewsController(APIContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    // POST: api/Reviews
    [HttpPost("/addReview")]
    public async Task<IActionResult> AddReview([FromBody] ReviewRequest reviewRequest)
    {
        if (reviewRequest == null)
        {
            return BadRequest("Requisição inválida.");
        }

        if (string.IsNullOrWhiteSpace(reviewRequest.MovieName))
        {
            return BadRequest("Nome do filme é obrigatório.");
        }

        // Verificar se o filme existe no banco de dados
        Movie existingMovie = null;
        try
        {
            existingMovie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Title.ToLower() == reviewRequest.MovieName.ToLower());
        }

        catch (Exception ex)
        {
            // Log de erro ou outras ações
            return StatusCode(500, $"Erro ao buscar filme: {ex.Message}");
        }

        if (existingMovie == null)
        {
            // Se o filme não existe, buscar na OMDb
            existingMovie = await GetMovieFromOmdbAsync(reviewRequest.MovieName);

            if (existingMovie == null) return NotFound("Filme não encontrado na OMDb.");  

            // Adicionar o filme ao banco de dados
            _context.Movie.Add(existingMovie);
            await _context.SaveChangesAsync();
        }

        // Criar a review associada ao filme
        var review = new Review
        {
            Rating = reviewRequest.Rating,
            Comment = reviewRequest.Comment,
            MovieTitle = existingMovie.Title,
            MovieId = existingMovie.Id,
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok(review);
    }

    private async Task<Movie> GetMovieFromOmdbAsync(string movieName)
    {
        var response = await _httpClient.GetAsync($"http://www.omdbapi.com/?t={movieName}&apikey={_omdbApiKey}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var omdbMovie = await response.Content.ReadFromJsonAsync<OMDbMovieResponse>();

        if (omdbMovie == null || omdbMovie.Title == null) return null;
        
        DateTime date;
        string format = "dd MMM yyyy";
        bool success = DateTime.TryParseExact(omdbMovie.Released, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

        Movie movie = new Movie
        {
            Title = omdbMovie.Title,
            Director = omdbMovie.Director,
            ReleaseDate = date,
            Genre = omdbMovie.Genre
        };
        return movie;
    }

    // GET: api/Reviews/movie/{movieId}
    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByMovie(int movieId)
    {
        // Verifica se o filme existe
        var movie = await _context.Movie.FindAsync(movieId);
        if (movie == null) return NotFound($"Movie with ID {movieId} not found.");
        
        // Obtém todas as reviews associadas ao filme
        var reviews = await _context.Reviews
            .Where(r => r.MovieId == movieId)
            .ToListAsync();

        return Ok(reviews);
    }
}
