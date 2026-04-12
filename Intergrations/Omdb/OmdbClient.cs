using JETechnicalAssessment.Intergrations.Models;
using JETechnicalAssessment.Models;
using System.Text.Json;

namespace JETechnicalAssessment.Intergrations.Omdb;

public class OmdbClient : IOmdbClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OmdbClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Omdb:ApiKey"] ?? throw new Exception("ApiKey missing");
    }

    public async Task<SearchResults?> SearchMoviesAsync(string query)
        => await SendRequestAsync<SearchResults>($"?apikey={_apiKey}&s={query}");

    public async Task<MovieDetails?> GetMovieDetailsAsync(string imdbId)
        => await SendRequestAsync<MovieDetails>($"?apikey={_apiKey}&i={imdbId}&plot=full");

    private async Task<T> SendRequestAsync<T>(string url) where T : OmdbBaseResponse, new()
    {
        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new T { Response = "False", Error = $"Server Error: {response.StatusCode}" };

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new T { Response = "False", Error = "Could not parse API response." };
        }
        catch (Exception)
        {
            return new T { Response = "False", Error = $"Unexpected Omdb error occurred." };
        }
    }
}