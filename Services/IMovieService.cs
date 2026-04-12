using JETechnicalAssessment.Models;

namespace JETechnicalAssessment.Services;

public interface IMovieService
{
    Task<OmdbSearchResults> SearchMoviesAsync(string query);

    Task<MovieDetails?> GetMovieDetailsAsync(string imdbId);
}
