namespace Intelligent_Support_Automation_System.Domain.DomainModels;

public class KnowledgeDocument : BaseEntity //Represents documents stored in Cognitive Search or our knowledge base.
{
    public string? QuestionId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Source { get; set; } //file name, URL, index field
}