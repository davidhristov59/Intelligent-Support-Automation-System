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
    /*
     *This method creates a new chat session and persists it to Cosmos DB where it is immediately available for use after creation.
     */
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
    /*
     * This method validates that the session exists, assigns a unique ID to the question,
        and adds it to the session's questions collection, where it updates the session's last message and timestamp,
        and persists the changes to Cosmos DB.
     */
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
    /*
     * This method performs a multi-step process
       1. Validates the question content 
       2. Searches the knowledge base for relevant documents using the question content
       3. Uses Azure OpenAI to generate a contextual response based on search results
       4. Creates a response object with unique ID and current timestamp
       5. Adds the response to the session and persists changes to Cosmos DB
       The method integrates search, AI generation, and data persistence in a single operation.
     */
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
    /*
     * This method provides safe retrieval of user chat sessions,
     * otherwise queries Cosmos DB for all sessions belonging to the specified user.
     */
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new List<ChatSession>();
        }

        return await _cosmosDbRepository.GetUserSessionsAsync(userId);
    }
}