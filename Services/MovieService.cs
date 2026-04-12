using JETechnicalAssessment.Intergrations.Omdb;
using JETechnicalAssessment.Models;

namespace JETechnicalAssessment.Services;

public class MovieService(IOmdbClient omdbClient) : IMovieService
{
    public async Task<OmdbSearchResults> SearchMoviesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new OmdbSearchResults { Response = "False" };

        var result = await omdbClient.SearchMoviesAsync(query);

        // 2. Business Logic: If the search was successful, save it to the DB
        if (result != null && result.Response == "True")
        {
            // We don't 'await' this if we don't want to block the UI, 
            // but for a reliable history, we await it here.
            //await _historyRepo.UpsertSearchAsync(title);
        }

        return result ?? new OmdbSearchResults { Response = "False" };
    }

    public async Task<MovieDetails?> GetMovieDetailsAsync(string imdbId)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return null;

        return await omdbClient.GetMovieDetailsAsync(imdbId);
    }
}
