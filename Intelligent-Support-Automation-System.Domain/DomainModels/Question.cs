namespace Intelligent_Support_Automation_System.Domain.DomainModels;

public class Question : BaseEntity //Represents a user's question within a chat session.
{
    public string? SessionId { get; set; }
    public string? Content { get; set; }
    public DateTime AskedAt { get; set; }
    // public Response? Response { get; set; }
}