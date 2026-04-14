using JETechnicalAssessment.Intergrations.Omdb;
using JETechnicalAssessment.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace JETechnicalAssessmentTests.Integrations;

public class OmdbClientTests
{
    private Mock<HttpMessageHandler> _handlerMock = null!;
    private Mock<IConfiguration> _configMock = null!;
    private OmdbClient _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _configMock = new Mock<IConfiguration>();

        _configMock.Setup(c => c["Omdb:ApiKey"]).Returns("test-key");

        var httpClient = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("https://test.com/")
        };

        _sut = new OmdbClient(httpClient, _configMock.Object);
    }

    [Test]
    public async Task SearchMoviesAsync_WhenApiReturnsSuccess_ReturnsData()
    {
        // Arrange
        var  _positiveResponse = new SearchResults()
        {
            Response = "True",
            totalResults = "1",
            Search = [new Search() { imdbID = "tt1234567" }]
        };

        var json = JsonSerializer.Serialize(_positiveResponse);

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        // Act
        var result = await _sut.SearchMoviesAsync("Batman");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Response, Is.EqualTo("True"));
        Assert.That(result.Search, Is.Not.Null);
        Assert.That(result.Search, Has.Count.GreaterThanOrEqualTo(1));
        Assert.That(result.Search[0].imdbID, Is.EqualTo("tt1234567"));
    }

    [Test]
    public async Task SearchMoviesAsync_WhenHttpStatusIsNotSuccessful_ReturnsServerError()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("")
            });

        // Act
        var result = await _sut.SearchMoviesAsync("Batman");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Response, Is.EqualTo("False"));
        Assert.That(result.Error, Does.Contain("Server Error"));
    }

    [Test]
    public async Task SearchMoviesAsync_WhenJsonDeserializesToNull_ReturnsParseError()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("null")
            });

        // Act
        var result = await _sut.SearchMoviesAsync("Batman");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Response, Is.EqualTo("False"));
        Assert.That(result.Error, Is.EqualTo("Could not parse API response."));
    }

    [Test]
    public async Task SearchMoviesAsync_WhenHttpClientThrows_ReturnsUnexpectedError()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("network failure"));

        // Act
        var result = await _sut.SearchMoviesAsync("Batman");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Response, Is.EqualTo("False"));
        Assert.That(result.Error, Is.EqualTo("Unexpected Omdb error occurred."));
    }

    [Test]
    public async Task GetMovieDetailsAsync_WhenApiReturnsSuccess_ReturnsData()
    {
        // Arrange
        var imdbId = "tt1375666";
        var expectedResponse = new MovieDetails
        {
            Response = "True",
            imdbID = imdbId,
            Title = "Inception",
            Year = "2010"
        };

        var json = JsonSerializer.Serialize(expectedResponse);

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        // Act
        var result = await _sut.GetMovieDetailsAsync(imdbId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Response, Is.EqualTo("True"));
        Assert.That(result.imdbID, Is.EqualTo(imdbId));
        Assert.That(result.Title, Is.EqualTo("Inception"));
        Assert.That(result.Year, Is.EqualTo("2010"));
    }

    [Test]
    public async Task GetMovieDetailsAsync_WhenHttpStatusIsNotSuccessful_ReturnsServerError()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("")
            });

        // Act
        var result = await _sut.GetMovieDetailsAsync("tt1375666");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Response, Is.EqualTo("False"));
        Assert.That(result.Error, Does.Contain("Server Error"));
    }

    [Test]
    public async Task GetMovieDetailsAsync_WhenJsonDeserializesToNull_ReturnsParseError()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("null")
            });

        // Act
        var result = await _sut.GetMovieDetailsAsync("tt1375666");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Response, Is.EqualTo("False"));
        Assert.That(result.Error, Is.EqualTo("Could not parse API response."));
    }

    [Test]
    public async Task GetMovieDetailsAsync_WhenHttpClientThrows_ReturnsUnexpectedError()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("network failure"));

        // Act
        var result = await _sut.GetMovieDetailsAsync("tt1375666");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Response, Is.EqualTo("False"));
        Assert.That(result.Error, Is.EqualTo("Unexpected Omdb error occurred."));
    }
}
