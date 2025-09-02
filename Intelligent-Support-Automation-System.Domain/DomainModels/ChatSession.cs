namespace Intelligent_Support_Automation_System.Domain.DomainModels;

public class ChatSession : BaseEntity //Represents a chat session between a user and the intelligent support system.
{ 
    public string? UserId { get; set; }
    public string? UserMessage { get; set; }
    public DateTime Timestamp { get; set; }
    public List<Question>? Questions { get; set; }
}