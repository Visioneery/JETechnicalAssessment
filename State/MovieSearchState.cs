using JETechnicalAssessment.Models;

namespace JETechnicalAssessment.State;

// Using just model state because it provides instant UI restoration without the overhead of JSON serialization/deserialization required by LocalStorage
public class MovieSearchState
{
    public string LastQuery { get; set; } = string.Empty;
    public SearchResults? LastResults { get; set; }
}