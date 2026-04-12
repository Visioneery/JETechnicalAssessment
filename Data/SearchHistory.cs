namespace JETechnicalAssessment.Data;

public class SearchHistory
{
    public int Id { get; set; }
    public string Query { get; set; } = string.Empty;
    public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
}