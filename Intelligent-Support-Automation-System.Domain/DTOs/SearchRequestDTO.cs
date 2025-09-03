namespace Intelligent_Support_Automation_System.Domain.DTOs;

public class SearchRequestDTO
{
    public string? Query { get; set; }
    public int MaxResults { get; set; }
}