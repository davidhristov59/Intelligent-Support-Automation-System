using Intelligent_Support_Automation_System.Domain.DomainModels;
using Intelligent_Support_Automation_System.Repository.Interface;
using Microsoft.Azure.Cosmos;
using Container = Microsoft.Azure.Cosmos.Container;
using Microsoft.Extensions.Configuration;

namespace Intelligent_Support_Automation_System.Repository.Implementation;

public class CosmosDbRepository : ICosmosDbRepository
{
    private readonly Container _container;

    public CosmosDbRepository(IConfiguration config)
    {
        var endpoint = config["CosmosDb:Endpoint"];
        var key = config["CosmosDb:Key"];
        var databaseName = config["CosmosDb:DatabaseName"];
        var containerName = config["CosmosDb:ContainerName"];

        var client = new CosmosClient(endpoint, key);
        _container = client.GetContainer(databaseName, containerName);
    }

    public async Task<ChatSession> GetSessionByIdAsync(string sessionId)
    /*
     * This method asynchronously retrieves a ChatSession from the Cosmos DB container by its session ID
     */
    {
        try
        {
            var response = await _container.ReadItemAsync<ChatSession>(sessionId, new PartitionKey(sessionId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<List<ChatSession>> GetUserSessionsAsync(string userId)
    /*
     * This method asynchronously retrieves all ChatSession objects for a given user from the Cosmos DB container where it
     * constructs a SQL query to select sessions where the userId matches the provided value
     */
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId")
            .WithParameter("@userId", userId);

        var iterator = _container.GetItemQueryIterator<ChatSession>(query);
        var sessions = new List<ChatSession>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            sessions.AddRange(response);
        }

        return sessions;
    }
    
    public async Task CreateSessionAsync(ChatSession session)
    /*
     * This method asynchronously creates a new ChatSession item in the Azure Cosmos DB container.
     * It uses the session's ID as the partition key to ensure proper data distribution and storage.
     */
    {
        // session.Id = Guid.NewGuid().ToString();
        await _container.CreateItemAsync(session, new PartitionKey(session.Id.ToString()));
    }

    public async Task SaveSessionAsync(ChatSession session)
        /*
         * This method asynchronously saves a ChatSession object to the Azure Cosmos DB container
         * which ensures that the session is stored and partitioned correctly in the database.
         */
    {
        await _container.UpsertItemAsync(session, new PartitionKey(session.Id.ToString()));
    }
    
    public async Task UpdateSessionAsync(ChatSession session)
        /*
         * Asynchronously updates an existing ChatSession in the Cosmos DB container.
         */    
    {
        try
        { 
            await _container.ReplaceItemAsync(session, session.Id.ToString(), new PartitionKey(session.Id.ToString()));
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException($"Session with ID {session.Id} not found for update.", ex);
        }
    }
}