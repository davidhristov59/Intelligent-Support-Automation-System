using Intelligent_Support_Automation_System.Domain.DomainModels;
using Intelligent_Support_Automation_System.Domain.DTOs;
using Intelligent_Support_Automation_System.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_Support_Automation_System.Web.controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    
    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }
    
    // POST: api/chat/sessions
    [HttpPost("sessions")] 
    public async Task<ActionResult<ChatSessionDTO>> StartChatSession([FromQuery] string userId) 
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("UserId is required.");
        
        var session = await _chatService.StartChatSessionAsync(userId);

        var sessionDTO = new ChatSessionDTO()
        {
            SessionId = session.Id.ToString(),
            UserId = session.UserId,
            StartedAt = session.Timestamp
        };
        
        return Ok(sessionDTO);
    }
    
    // POST: api/chat/sessions/{sessionId}/ask
    [HttpPost("sessions/{sessionId}/ask")]
    public async Task<ActionResult<AskQuestionResponse>> AskQuestion(string sessionId, [FromBody] AskQuestionRequest request) 
    { 
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Question content is required.");

        var question = new Question()
        {
            SessionId = sessionId,
            Content = request.Content,
            AskedAt = DateTime.UtcNow
        };

        var savedQuestion = await _chatService.AskQuestionAsync(question);
        var response = await _chatService.GenerateResponseAsync(savedQuestion);

        var result = new AskQuestionResponse
        {
            Question = new QuestionDTO
            {
                QuestionId = savedQuestion.Id.ToString(),
                Content = savedQuestion.Content,
                AskedAt = savedQuestion.AskedAt
            },
            Response = new ResponseDTO()
            {
                ResponseId = response.Id.ToString(),
                Content = response.Content,
                GeneratedAt = response.GeneratedAt
            }
        };
        
        return Ok(result);
    }

    // POST: api/chat/generate-response
    [HttpPost("generate-response")]
    public async Task<ActionResult<ResponseDTO>> GenerateResponse([FromBody] QuestionDTO questionDTO)
    {
        var question = new Question()
        {
            Id = Guid.Parse(questionDTO.QuestionId),
            Content = questionDTO.Content,
            AskedAt = questionDTO.AskedAt,
        };

        var response = await _chatService.GenerateResponseAsync(question);

        var responseDTO = new ResponseDTO()
        {
            ResponseId = response.Id.ToString(),
            Content = response.Content,
            GeneratedAt = response.GeneratedAt
        };
        return Ok(responseDTO);
    }
    
    // GET: api/chat/sessions/{userId}
    [HttpGet("sessions/{userId}")]
    public async Task<ActionResult<List<ChatSession>>> GetUserSessions (string userId)
    {
        var sessions = await _chatService.GetChatSessionsByUserIdAsync(userId);

        var dtos = sessions.Select(s => new ChatSessionDTO()
        {
            SessionId = s.Id.ToString(),
            UserId = s.UserId,
            StartedAt = s.Timestamp
        }).ToList();

        return Ok(dtos);
    }
    
    // GET: api/chat/sessions/details/{sessionId}
    [HttpGet("sessions/details/{sessionId}")]
    public async Task<ActionResult<ChatSessionDTO>> GetSessionDetails(string sessionId)
    {
        var session = await _chatService.GetSessionByIdAsync(sessionId);

        var dto = new ChatSessionDTO()
        {
            SessionId = session.Id.ToString(),
            StartedAt = session.Timestamp,
            UserId = session.UserId
        };

        return Ok(dto);
    }
}