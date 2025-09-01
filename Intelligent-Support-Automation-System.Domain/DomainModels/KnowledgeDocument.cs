namespace Intelligent_Support_Automation_System.Domain.DomainModels;

public class KnowledgeDocument : BaseEntity //Represents documents stored in Cognitive Search or our knowledge base.
{
    public string? QuestionId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }

    public KnowledgeDocument(string? questionId, string? title, string? content, string? category, DateTime createdAt)
    {
        QuestionId = questionId;
        Title = string.Empty;
        Content = string.Empty;
        Category = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }
}