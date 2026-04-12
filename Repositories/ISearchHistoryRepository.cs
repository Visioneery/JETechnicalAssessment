namespace JETechnicalAssessment.Repositories;

public interface ISearchHistoryRepository
{
    Task SaveSearchAsync(string query);

    Task<List<string>> GetRecentQueriesAsync();
}
