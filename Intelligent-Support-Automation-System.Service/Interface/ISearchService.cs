using Intelligent_Support_Automation_System.Domain.DomainModels;

namespace Intelligent_Support_Automation_System.Service.Interface;

/**
 * interacts with our knowledge base (KnowledgeDocument) to fetch relevant answers.
 */
public interface ISearchService
{
    Task<List<KnowledgeDocument>> SearchKnowledgeBaseAsync(string query, int maxResults=5);
}