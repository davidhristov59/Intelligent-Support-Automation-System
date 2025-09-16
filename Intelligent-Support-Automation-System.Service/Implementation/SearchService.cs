using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Intelligent_Support_Automation_System.Domain.DomainModels;
using Intelligent_Support_Automation_System.Domain.DTOs;
using Intelligent_Support_Automation_System.Service.Interface;
using Microsoft.Extensions.Configuration;

namespace Intelligent_Support_Automation_System.Service.Implementation;

public class SearchService : ISearchService
{
    private readonly SearchClient _searchClient;
    
    public SearchService(IConfiguration config)
    {
        var endpoint = config["AzureSearch:Endpoint"];
        var indexName = config["AzureSearch:IndexName"];
        var apiKey = config["AzureSearch:ApiKey"];

        _searchClient = new SearchClient(new Uri(endpoint), indexName, new AzureKeyCredential(apiKey));
    }
    
    public async Task<List<KnowledgeDocument>> SearchKnowledgeBaseAsync(SearchRequestDTO request)
    /*
     * This method performs a full-text search against the configured Azure Search index and maps
     * the search results to domain objects. It extracts fields (title, content, category, source)
     * from the search documents, handling missing fields gracefully by providing default values.
     */
    {
        var searchOptions = new SearchOptions()
        {
            Size = request.MaxResults,
            Select = { "id", "title", "content", "category", "source_document", "chunk_id" }
        };
        
        var filters = new List<string>();
    
        if (!string.IsNullOrEmpty(request.Category))
        {
            filters.Add($"category eq '{request.Category}'");
        }
    
        if (!string.IsNullOrEmpty(request.SourceDocument))
        {
            filters.Add($"source_document eq '{request.SourceDocument}'");
        }
    
        if (filters.Any())
        {
            searchOptions.Filter = string.Join(" and ", filters);
        }

        var response = await _searchClient.SearchAsync<SearchDocument>(request.Query, searchOptions);

        var results = new List<KnowledgeDocument>();

        await foreach (var result in response.Value.GetResultsAsync())
        {
            if (request.MinimumScore.HasValue && result.Score < request.MinimumScore.Value)
                continue;

            var document = new KnowledgeDocument
            {
                QuestionId = result.Document.TryGetValue("id", out var id)
                    ? id?.ToString()
                    : Guid.NewGuid().ToString(),

                Title = result.Document.TryGetValue("title", out var title)
                    ? title?.ToString()
                    : string.Empty,

                Content = result.Document.TryGetValue("content", out var content)
                    ? content?.ToString()
                    : string.Empty,

                Category = result.Document.TryGetValue("category", out var category)
                    ? category?.ToString()
                    : string.Empty,

                SourceDocument = result.Document.TryGetValue("source_document", out var sourceDoc)
                    ? sourceDoc?.ToString()
                    : string.Empty,

                ChunkId = result.Document.TryGetValue("chunk_id", out var chunkId)
                    ? chunkId?.ToString()
                    : string.Empty,

                CreatedAt = DateTime.UtcNow
            };
            results.Add(document);
        }
        return results;
    }
}