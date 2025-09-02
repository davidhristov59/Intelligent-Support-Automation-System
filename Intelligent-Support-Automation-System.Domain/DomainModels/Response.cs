namespace Intelligent_Support_Automation_System.Domain.DomainModels;

public class Response : BaseEntity //Represents the system's answer to a user's question.
{
    public string? QuestionId { get; set; }
    public string? SessionId { get; set; }
    public string? Content { get; set; }
    public DateTime GeneratedAt { get; set; }
}