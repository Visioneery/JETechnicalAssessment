using JETechnicalAssessment.Data;
using JETechnicalAssessment.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JETechnicalAssessmentTests.Repositories;

public class SearchHistoryRepositoryTests
{
    private SqliteConnection _connection;
    private AppDbContext _dbContext;
    private SearchHistoryRepository _sut;

    [SetUp]
    public async Task SetUp()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new AppDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        _sut = new SearchHistoryRepository(_dbContext);
    }

    [Test]
    public async Task SaveSearchAsync_WhenDuplicateExists_StillKeepsOnlyLastFiveItems()
    {
        // Arrange
        _dbContext.SearchHistories.AddRange(
            new SearchHistory { Query = "Inception", SearchedAt = DateTime.UtcNow.AddMinutes(-6) },
            new SearchHistory { Query = "Alien", SearchedAt = DateTime.UtcNow.AddMinutes(-5) },
            new SearchHistory { Query = "Batman", SearchedAt = DateTime.UtcNow.AddMinutes(-4) },
            new SearchHistory { Query = "Superman", SearchedAt = DateTime.UtcNow.AddMinutes(-3) },
            new SearchHistory { Query = "Matrix", SearchedAt = DateTime.UtcNow.AddMinutes(-2) },
            new SearchHistory { Query = "Joker", SearchedAt = DateTime.UtcNow.AddMinutes(-1) }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.SaveSearchAsync("Batman");

        // Assert
        var items = await _dbContext.SearchHistories
            .AsNoTracking()
            .OrderByDescending(x => x.SearchedAt)
            .ToListAsync();


        Assert.That(items, Has.Count.EqualTo(5));
        Assert.That(items[0].Query, Is.EqualTo("Batman"));
        Assert.That(items.Count(x => x.Query == "Batman"), Is.EqualTo(1));
        Assert.That(items.Any(x => x.Query == "Inception"), Is.False);
    }


    [Test]
    public async Task GetRecentQueriesAsync_ReturnsQueriesInDescendingOrder()
    {
        _dbContext.SearchHistories.AddRange(
            new SearchHistory { Query = "First", SearchedAt = DateTime.UtcNow.AddMinutes(-2) },
            new SearchHistory { Query = "Second", SearchedAt = DateTime.UtcNow.AddMinutes(-1) },
            new SearchHistory { Query = "Third", SearchedAt = DateTime.UtcNow }
        );

        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetRecentQueriesAsync();

        Assert.That(result, Is.EqualTo(new[] { "Third", "Second", "First" }));
    }

    [TearDown]
    public async Task TearDown()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }
}