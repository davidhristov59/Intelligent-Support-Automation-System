using Intelligent_Support_Automation_System.Domain.DomainModels;
using Intelligent_Support_Automation_System.Repository.Interface;
using Intelligent_Support_Automation_System.Service.Interface;

namespace Intelligent_Support_Automation_System.Service.Implementation;

// Service to manage chat sessions, questions, and responses
public class ChatService : IChatService
{
    private readonly ISearchService _searchService;
    private readonly ICosmosDbRepository _cosmosDbRepository;
    private readonly AzureOpenAIService _azureOpenAIService;

    public ChatService(ISearchService searchService, ICosmosDbRepository cosmosDbRepository, AzureOpenAIService azureOpenAiService)
    {
        _searchService = searchService;
        _cosmosDbRepository = cosmosDbRepository;
        _azureOpenAIService = azureOpenAiService;
    }

    public async Task<ChatSession> StartChatSessionAsync(string userId)
    {
        var session = new ChatSession()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            Questions = new List<Question>(),
            Responses = new List<Response>(),
            UserMessage = string.Empty
        };

        await _cosmosDbRepository.SaveSessionAsync(session);
        return session;
    }

    public async Task<Question> AskQuestionAsync(Question question)
    {
        var session = await _cosmosDbRepository.GetSessionByIdAsync(question.SessionId);
        
        if (session == null)
        {
            throw new InvalidOperationException($"Chat session with ID {question.SessionId} not found.");
        }

        question.Id = Guid.NewGuid();
        question.AskedAt = DateTime.UtcNow;
        
        session.Questions ??= new List<Question>(); //ensures that the Questions list is initialized if it is null
        session.Questions.Add(question);

        session.UserMessage = question.Content;
        session.Timestamp = DateTime.UtcNow;

        await _cosmosDbRepository.SaveSessionAsync(session);

        return question;
    }

    public async Task<Response> GenerateResponseAsync(Question question)
    {
        if (string.IsNullOrEmpty(question.Content))
        {
            return new Response()
            {
                Id = Guid.NewGuid(),
                QuestionId = question.Id.ToString(),
                GeneratedAt = DateTime.UtcNow,
                Content = "Please provide a valid question.",
                SessionId = question.SessionId
            };
        }

        //Call SearchService to retrieve relevant knowledge base articles
        var searchResults = await _searchService.SearchKnowledgeBaseAsync(question.Content, 5);
        
        //Feed search results and question to OpenAI API to generate a response
        var prompt = await _azureOpenAIService.GenerateResponseAsync(question.Content, searchResults);
        
        var response = new Response()
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id.ToString(),
            GeneratedAt = DateTime.UtcNow,
            Content = prompt,
            SessionId = question.SessionId
        };

        //Save the response into the session
        var session = await _cosmosDbRepository.GetSessionByIdAsync(question.SessionId);
        session.Responses ??= new List<Response>();
        session.Responses.Add(response);

        await _cosmosDbRepository.SaveSessionAsync(session);

        return response;
    }

    public async Task<List<ChatSession>> GetChatSessionsByUserIdAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new List<ChatSession>();
        }

        return await _cosmosDbRepository.GetUserSessionsAsync(userId);
    }
}