using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Indexer;

public class Ingestor
{
    private readonly CosmosClient _cosmosClient;
    //private static readonly CosmosClientOptions _cosmosClientOptions = new()
    //{
    //    SerializerOptions = new CosmosSerializationOptions
    //    {
    //        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
    //    }
    //};
    //private static readonly CosmosClient _cosmosClient
    //    = new(Environment.GetEnvironmentVariable("CosmosConnection") ?? throw new NullReferenceException("CosmosConnection"), _cosmosClientOptions);

    public Ingestor(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    [FunctionName("Ingestor")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null), FromBody] AzureInfo[] input,
        ILogger log)
    {
        var container = _cosmosClient.GetContainer("aoai", "azure");

        var tasks = input.Select(item => container.UpsertItemAsync(item, new PartitionKey(item.Id))).ToArray();
        await Task.WhenAll(tasks);

        return new OkObjectResult($"{input.Length} items uploaded");
    }
}