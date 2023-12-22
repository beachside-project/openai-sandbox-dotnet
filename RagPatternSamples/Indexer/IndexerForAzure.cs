using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Indexer;

public class IndexerForAzure
{
    private readonly OpenAIClient _aiClient;
    private readonly SearchClient _searchClient;
    private static readonly string DeploymentNameAda = "text-embedding-ada-002";

    //// TODO: DI
    //private static readonly OpenAIClient _aiClient = new(
    //    new Uri(Environment.GetEnvironmentVariable("AzureOpenAIOptions:Endpoint")),
    //    new AzureKeyCredential(Environment.GetEnvironmentVariable("AzureOpenAIOptions:ApiKey")));

    //// TODO: DI
    //private static readonly SearchClientOptions SearchClientOptions = new()
    //{
    //    Serializer = new JsonObjectSerializer(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
    //};

    //private static readonly SearchClient _searchClient = new(
    //    new Uri(Environment.GetEnvironmentVariable("AISearchOptions:Endpoint")),
    //    Environment.GetEnvironmentVariable("AISearchOptions:IndexName"),
    //    new AzureKeyCredential(Environment.GetEnvironmentVariable("AISearchOptions:AdminKey")),
    //    SearchClientOptions
    //);

    public IndexerForAzure(OpenAIClient aiClient, SearchClient searchClient)
    {
        _aiClient = aiClient;
        _searchClient = searchClient;
    }

    [FunctionName("Function1")]
    public async Task Run([CosmosDBTrigger(
            databaseName: "aoai",
            containerName: "azure",
            Connection = "CosmosConnection",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)]IReadOnlyList<AzureInfo> input,
        ILogger log)
    {
        //var documentsToUpload = new List<AzureDoc>();

        //foreach (var data in input)
        //{
        //    var contentVector = await GetVectorAsync(data.Content);

        //    documentsToUpload.Add(new AzureDoc
        //    {
        //        Id = data.Id,
        //        Category = data.Category,
        //        Title = data.Title,
        //        Content = data.Content,
        //        ContentVector = contentVector
        //    });
        //}

        //await _searchClient.MergeOrUploadDocumentsAsync(documentsToUpload);
        log.LogInformation($"{input.Count} document(s) uploaded");
    }

    private async Task<IReadOnlyList<float>> GetVectorAsync(string text)
    {
        var options = new EmbeddingsOptions
        {
            DeploymentName = DeploymentNameAda,
            Input = { text }
        };

        var response = await _aiClient.GetEmbeddingsAsync(options);
        var vector = response.Value.Data[0].Embedding;
        return vector.ToArray();
    }
}

// Customize the model with your own desired properties