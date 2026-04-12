using JETechnicalAssessment.Models;

namespace JETechnicalAssessment.Intergrations.Omdb;

public interface IOmdbClient
{
    Task<SearchResults?> SearchMoviesAsync(string query);

    Task<MovieDetails?> GetMovieDetailsAsync(string imdbId);
}
