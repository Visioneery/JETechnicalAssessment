using JETechnicalAssessment.Models;

namespace JETechnicalAssessment.Services;

public interface IMovieService
{
    Task<SearchResults> SearchMoviesAsync(string query);

    Task<MovieDetails?> GetMovieDetailsAsync(string imdbId);

    Task<List<string>> GetRecentSearchesAsync();
}
