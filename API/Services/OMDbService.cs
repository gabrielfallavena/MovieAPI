using API.Models;

namespace API.Services;
public class OMDbService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "21eaea2"; 

    public OMDbService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Movie> GetMovieByTitleAsync(string title)
    {
        var response = await _httpClient.GetAsync($"http://www.omdbapi.com/?t={title}&apikey={_apiKey}");
        if (response.IsSuccessStatusCode)
        {
            var movieData = await response.Content.ReadFromJsonAsync<OMDbMovieResponse>();

            // Mapear para o seu modelo Movie
            return new Movie
            {
                Title = movieData.Title,
                Director = movieData.Director,
                Genre = movieData.Genre,
                ReleaseDate = DateTime.Parse(movieData.Released)
            };
        }
        return null;
    }
}