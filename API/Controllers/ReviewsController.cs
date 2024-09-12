using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

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

    // Adiciona uma review no banco de dados
    // Se o filme não estiver no banco, adiciona-o.
    // Se não, apenas adiciona a review ao filme 
    // POST: api/Reviews
    [Authorize]
    [HttpPost("/addReview")]
    public async Task<IActionResult> AddReview([FromBody] ReviewRequest reviewRequest)
    {
        // Obter o email do usuário autenticado a partir do token JWT
        var userEmail = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (userEmail == null) return Unauthorized("Usuário não autenticado.");

        // Buscar o usuário no banco de dados usando o email
        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized("Usuário não encontrado.");

        if (reviewRequest == null) return BadRequest("Requisição inválida.");
        
        if (string.IsNullOrWhiteSpace(reviewRequest.MovieName)) return BadRequest("Nome do filme é obrigatório.");

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
            UserId = user.Id            
        };

        // Adiciona no banco de dados e salva
        _context.Review.Add(review);
        user.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok(review);
    }

    // Procura o filme no OMDb a partir do seu nome, retorna um Movie
    private async Task<Movie> GetMovieFromOmdbAsync(string movieName)
    {
        var response = await _httpClient.GetAsync($"http://www.omdbapi.com/?t={movieName}&apikey={_omdbApiKey}");

        if (!response.IsSuccessStatusCode) return null;

        var omdbMovie = await response.Content.ReadFromJsonAsync<OMDbMovieResponse>();

        if (omdbMovie == null || omdbMovie.Title == null) return null;
        
        // Preparo para formatar a data de lançamento
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

    // Busca as reviews do filme passado como parametro
    // GET: api/Reviews/movie/{movieTitle}
    [HttpGet("{movieTitle}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByMovie(string movieTitle)
    {
        // Verifica se o filme existe
        var movie = await _context.Movie.FirstOrDefaultAsync(m => m.Title == movieTitle);
        if (movie == null) return NotFound($"Movie with name {movieTitle} not found.");
        
        // Obtém todas as reviews associadas ao filme
        var reviews = await _context.Review
            .Where(r => r.MovieTitle == movieTitle)
            .ToListAsync();

        return Ok(reviews);
    }
}
