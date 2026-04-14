using JETechnicalAssessment.Data;
using Microsoft.EntityFrameworkCore;

namespace JETechnicalAssessment.Repositories;

public class SearchHistoryRepository(AppDbContext dbContext) : ISearchHistoryRepository
{
    public async Task SaveSearchAsync(string query)
    {
        var trimmedQuery = query.Trim();

        var existing = await dbContext.SearchHistories
            .FirstOrDefaultAsync(h => h.Query == trimmedQuery);

        if (existing != null)
        {
            dbContext.SearchHistories.Remove(existing);
        }

        dbContext.SearchHistories.Add(new SearchHistory
        {
            Query = trimmedQuery,
            SearchedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        var cutoffItem = await dbContext.SearchHistories
            .OrderByDescending(x => x.SearchedAt)
            .ThenByDescending(x => x.Id)
            .Skip(4)
            .FirstOrDefaultAsync();

        if (cutoffItem != null)
        {
            await dbContext.SearchHistories
                .Where(x => x.SearchedAt < cutoffItem.SearchedAt ||
                            (x.SearchedAt == cutoffItem.SearchedAt && x.Id < cutoffItem.Id))
                .ExecuteDeleteAsync();
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<List<string>> GetRecentQueriesAsync()
    {
        return await dbContext.SearchHistories
            .OrderByDescending(x => x.SearchedAt)
            .Select(x => x.Query)
            .ToListAsync();
    }
}