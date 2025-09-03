using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Intelligent_Support_Automation_System.Domain.DomainModels;
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
    
    public async Task<List<KnowledgeDocument>> SearchKnowledgeBaseAsync(string query, int maxResults = 5)
    /*
     * This method performs a full-text search against the configured Azure Search index and maps
     * the search results to domain objects. It safely extracts fields (title, content, category, source)
     * from the search documents, handling missing fields gracefully by providing default values.
     */
    {
        var searchOptions = new SearchOptions()
        {
            Size = maxResults
        };

        var response = await _searchClient.SearchAsync<SearchDocument>(query, searchOptions);

        var results = new List<KnowledgeDocument>();

        await foreach (var result in response.Value.GetResultsAsync())
        {

            results.Add(new KnowledgeDocument
            {
                Title = result.Document.TryGetValue("title", out var title)
                    ? title?.ToString()
                    : string.Empty,

                Content = result.Document.TryGetValue("content", out var value)
                    ? value?.ToString() ?? string.Empty
                    : string.Empty,

                Category = result.Document.TryGetValue("category", out var category)
                    ? category?.ToString()
                    : string.Empty,

                Source = result.Document.TryGetValue("source", out var source)
                    ? source?.ToString()
                    : string.Empty,

                CreatedAt = DateTime.UtcNow
            });
        }

        return results;
    }
}