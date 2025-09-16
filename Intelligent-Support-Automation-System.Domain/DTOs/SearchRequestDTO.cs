namespace Intelligent_Support_Automation_System.Domain.DTOs;

public class SearchRequestDTO
{
    public string? Query { get; set; }
    public int MaxResults { get; set; }
    public string? Category { get; set; } //filtering by category
    public string? SourceDocument { get; set; } //filtering by source document
    public double? MinimumScore { get; set; } = 0.1; //minimum relevance score to include results
}