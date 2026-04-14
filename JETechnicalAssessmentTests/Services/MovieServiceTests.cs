using JETechnicalAssessment.Intergrations.Omdb;
using JETechnicalAssessment.Models;
using JETechnicalAssessment.Repositories;
using JETechnicalAssessment.Services;
using Moq;

namespace JETechnicalAssessmentTests.Services;

public class MovieServiceTests
{
    private Mock<IOmdbClient> _omdbClientMock;
    private Mock<ISearchHistoryRepository> _repoMock;
    private MovieService _sut;

    [SetUp]
    public void Setup()
    {
        _omdbClientMock = new Mock<IOmdbClient>(MockBehavior.Strict);
        _repoMock = new Mock<ISearchHistoryRepository>(MockBehavior.Strict);

        _sut = new MovieService(_omdbClientMock.Object, _repoMock.Object);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task SearchMoviesAsync_WithInvalidQuery_ReturnsFalseAndDoesNotCallDependencies(string? query)
    {
        // Act
        var result = await _sut.SearchMoviesAsync(query!);

        // Assert
        Assert.That(result.Response, Is.EqualTo("False"));
        _omdbClientMock.VerifyNoOtherCalls();
        _repoMock.VerifyNoOtherCalls();
    }

    [Test, TestCaseSource(nameof(FailureScenarios))]
    public async Task SearchMoviesAsync_WhenOmdbReturnsFailure_ReturnsFalseAndDoesNotSaveQuery(SearchResults? apiResponse)
    {
        // Arrange
        var query = "Batman";
        _omdbClientMock
            .Setup(c => c.SearchMoviesAsync(query))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _sut.SearchMoviesAsync(query);

        // Assert
        Assert.That(result.Response, Is.EqualTo("False"));
        _repoMock.Verify(r => r.SaveSearchAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task SearchMoviesAsync_WhenOmdbReturnsSuccess_SavesQueryAndReturnsResults()
    {
        // Arrange
        var query = "Batman";
        var expected = new SearchResults
        {
            Response = "True",
            totalResults = "1",
            Search =
            [
                new Search
                {
                    Title = "Batman",
                    Year = "1989",
                }
            ]
        };

        _omdbClientMock
            .Setup(c => c.SearchMoviesAsync(query))
            .ReturnsAsync(expected);

        _repoMock
            .Setup(r => r.SaveSearchAsync(query))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.SearchMoviesAsync(query);

        // Assert
        Assert.That(result, Is.SameAs(expected));
        _omdbClientMock.Verify(c => c.SearchMoviesAsync(query), Times.Once);
        _repoMock.Verify(r => r.SaveSearchAsync(query), Times.Once);
        _omdbClientMock.VerifyNoOtherCalls();
        _repoMock.VerifyNoOtherCalls();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task GetMovieDetailsAsync_WithInvalidId_ReturnsNull(string? imdbId)
    {
        // Act
        var result = await _sut.GetMovieDetailsAsync(imdbId!);

        // Assert
        Assert.That(result, Is.Null);
        _omdbClientMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetMovieDetailsAsync_ReturnsDetailsFromOmdbClient()
    {
        // Arrange
        var imdbId = "tt1375666";
        var expected = new MovieDetails
        {
            imdbID = imdbId,
            Title = "Inception",
            Year = "2010"
        };

        _omdbClientMock
            .Setup(c => c.GetMovieDetailsAsync(imdbId))
            .ReturnsAsync(expected);

        // Act
        var result = await _sut.GetMovieDetailsAsync(imdbId);

        // Assert
        Assert.That(result, Is.SameAs(expected));
        _omdbClientMock.Verify(c => c.GetMovieDetailsAsync(imdbId), Times.Once);
        _omdbClientMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetRecentSearchesAsync_ReturnsQueriesFromRepository()
    {
        // Arrange
        var expected = new List<string> { "Batman", "Inception", "Interstellar" };

        _repoMock
            .Setup(r => r.GetRecentQueriesAsync())
            .ReturnsAsync(expected);

        // Act
        var result = await _sut.GetRecentSearchesAsync();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
        _repoMock.Verify(r => r.GetRecentQueriesAsync(), Times.Once);
        _repoMock.VerifyNoOtherCalls();
    }

    // Did not want to duplicate code so created some objects that are aviable at compile time
    private static IEnumerable<SearchResults?> FailureScenarios()
    {
        yield return null;
        yield return new SearchResults { Response = "False" };
    }
}
