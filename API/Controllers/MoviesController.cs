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
    private readonly string _omdbApiKey = "21eaea2"; // Insira sua chave da OMDb API aqui

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

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("Filme não encontrado na OMDb.");
        }

        var omdbMovie = await response.Content.ReadFromJsonAsync<OMDbMovieResponse>();

        if (omdbMovie == null || omdbMovie.Title == null) return NotFound("Filme não encontrado.");
     
        return Ok(omdbMovie);
    }
}