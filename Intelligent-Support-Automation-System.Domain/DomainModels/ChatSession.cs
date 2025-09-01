namespace Intelligent_Support_Automation_System.Domain.DomainModels;

public class ChatSession : BaseEntity //Represents a chat session between a user and the intelligent support system.
{ 
    public string? UserId { get; set; }
    public string? UserMessage { get; set; }
    public DateTime Timestamp { get; set; }
    public List<Question> Questions { get; set; }
    
    public ChatSession(string? userId, string? userMessage, DateTime timestamp, List<Question> questions)
    {
        UserId = userId;
        UserMessage = string.Empty;
        Timestamp = DateTime.UtcNow;
        Questions = new List<Question>();
    }
}