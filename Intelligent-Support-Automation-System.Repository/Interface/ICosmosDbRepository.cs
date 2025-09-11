using Intelligent_Support_Automation_System.Domain.DomainModels;

namespace Intelligent_Support_Automation_System.Repository.Interface;

public interface ICosmosDbRepository
{
    Task<ChatSession> GetSessionByIdAsync(string sessionId, string userId);
    Task<List<ChatSession>> GetUserSessionsAsync(string userId);
    Task CreateSessionAsync(ChatSession session);
    Task SaveSessionAsync(ChatSession session);
    Task UpdateSessionAsync(ChatSession session);
}