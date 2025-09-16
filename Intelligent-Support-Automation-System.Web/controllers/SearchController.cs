using Intelligent_Support_Automation_System.Domain.DTOs;
using Intelligent_Support_Automation_System.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_Support_Automation_System.Web.controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpPost]
    public async Task<ActionResult<List<SearchResultsDTO>>> SearchKnowledgeBase ([FromBody] SearchRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest("Search query is required.");

        if (request.MaxResults <= 0)
            request.MaxResults = 5;
        
        var results = await _searchService.SearchKnowledgeBaseAsync(request);

        var dtoResults = results.Select(s => new SearchResultsDTO()
        {
            Id = s.Id.ToString() ?? string.Empty,
            Title = s.Title ?? string.Empty,
            Content = s.Content ?? string.Empty,
            Category = s.Category ?? string.Empty,
            CreatedAt = s.CreatedAt
        }).ToList();

        return Ok(dtoResults);
    }
    
}