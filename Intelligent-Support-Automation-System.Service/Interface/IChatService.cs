using Intelligent_Support_Automation_System.Domain.DomainModels;

namespace Intelligent_Support_Automation_System.Service.Interface;

/**
 * handles chat sessions, questions, answers, and integrating external API/OpenAI responses.
 */
public interface IChatService
{
    Task<ChatSession> StartChatSessionAsync(string userId);
    Task<Question> AskQuestionAsync(Question question);
    Task<Response> GenerateResponseAsync(Question question);
    Task<List<ChatSession>> GetChatSessionsByUserIdAsync(string userId);
}