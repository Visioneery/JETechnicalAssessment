using JETechnicalAssessment.Intergrations.Omdb;
using JETechnicalAssessment.Models;
using JETechnicalAssessment.Repositories;

namespace JETechnicalAssessment.Services;

public class MovieService(IOmdbClient omdbClient, ISearchHistoryRepository searchHistoryRepository) : IMovieService
{
    public async Task<SearchResults> SearchMoviesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new SearchResults { Response = "False" };

        var result = await omdbClient.SearchMoviesAsync(query);

        if (result != null && result.Response == "True")
        {
            // awaiting unless such things happen in some hosted service or background queues ( depends on the requirements and use case )
            await searchHistoryRepository.SaveSearchAsync(query);
        }

        return result ?? new SearchResults { Response = "False" };
    }

    public async Task<MovieDetails?> GetMovieDetailsAsync(string imdbId)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return null;

        return await omdbClient.GetMovieDetailsAsync(imdbId);
    }

    public Task<List<string>> GetRecentSearchesAsync()
        => searchHistoryRepository.GetRecentQueriesAsync();
}
