namespace Intelligent_Support_Automation_System.Domain.DTOs;

//Response DTO for search results
public class SearchResultsDTO
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }
}