namespace Intelligent_Support_Automation_System.Domain.DTOs;

public class ChatSessionDTO
{
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
    public DateTime StartedAt { get; set; }
}