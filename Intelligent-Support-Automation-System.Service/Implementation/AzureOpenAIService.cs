using Azure;
using Azure.AI.OpenAI;
using Intelligent_Support_Automation_System.Domain.DomainModels;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace Intelligent_Support_Automation_System.Service.Implementation;

//
public class AzureOpenAIService
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentName;

    public AzureOpenAIService(IConfiguration config)
    {
        var endpoint = config["AzureOpenAI:Endpoint"];
        var apiKey = config["AzureOpenAI:ApiKey"];
        _deploymentName = config["AzureOpenAI:DeploymentName"];
        _client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    }

    public async Task<string> GenerateResponseAsync(string userMessage, List<KnowledgeDocument> searchResults)
    {
        //Set options for the chat completion
        var chatCompletionOptions = new ChatCompletionOptions()
        {
            MaxOutputTokenCount = 500,
            Temperature = 0.7f
        };

        //Create context from search results
        var context = string.Join("\n", searchResults.Select(doc => doc.Content));
        
        //Create messages for the chat completion
        var messages = new List<ChatMessage>()
        {
            ChatMessage.CreateSystemMessage(
                $"You are a helpful assistant. Use the following context: {context} to answer the user's question. If the context does not contain the answer, respond with 'I don't know'."),
            ChatMessage.CreateUserMessage(userMessage),
        };
        
        //Get chat client and make API calls
        var chatClient = _client.GetChatClient(_deploymentName);
        var response = await chatClient.CompleteChatAsync(messages, chatCompletionOptions);

        return response.Value.Content[0].Text; //Return the generated response text
    }
}