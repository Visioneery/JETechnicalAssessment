using JETechnicalAssessment.Intergrations.Models;

namespace JETechnicalAssessment.Models;

public class SearchResults : OmdbBaseResponse
{
    public List<Search> Search { get; set; } = [];
    public string totalResults { get; set; } = string.Empty;
}

public class Search
{
    public string Title { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string imdbID { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
}
