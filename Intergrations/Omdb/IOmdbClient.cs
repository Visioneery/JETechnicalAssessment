using JETechnicalAssessment.Models;

namespace JETechnicalAssessment.Intergrations.Omdb;

public interface IOmdbClient
{
    Task<OmdbSearchResults?> SearchMoviesAsync(string query);

    Task<MovieDetails?> GetMovieDetailsAsync(string imdbId);
}
