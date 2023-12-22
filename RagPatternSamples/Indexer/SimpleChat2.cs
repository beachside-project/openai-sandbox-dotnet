using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace Indexer;

public class SimpleChat2
{
    private const string DeploymentNameAda = "text-embedding-ada-002";

    private readonly OpenAIClient _aiClient;
    private readonly SearchClient _searchClient;

    public SimpleChat2(OpenAIClient aiClient, SearchClient searchClient)
    {
        _aiClient = aiClient;
        _searchClient = searchClient;
    }

    [FunctionName("SimpleChat2")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "chat/multi")]
        HttpRequest req,
        ILogger log)
    {
        var query = req.Query["query"];
        if (string.IsNullOrWhiteSpace(query))
        {
            return new BadRequestObjectResult("Please pass a 'query' on the query string or in the request body");
        }

        var references = await GetReferencesAsync(query);

        return new OkObjectResult(references);
    }

    private async Task<List<AzureInfo>> GetReferencesAsync(string query)
    {
        var vector = await GetVectorAsync(query);
        // 複数の fields を指定して、k=5の場合
        // それぞれの vector search で5件ずつ取得し、その論理和が最終的な response となるため、結果は5件以上になることがある。
        var vectorizedQuery = new VectorizedQuery(vector)
        {
            KNearestNeighborsCount = 5,
            Fields = { "contentVector", "titleVector" }
            //Fields = { "titleVector" }
            //Fields = { "contentVector" }
        };

        var searchOptions = new SearchOptions
        {
            VectorSearch = new VectorSearchOptions
            {
                Queries = { vectorizedQuery }
            },
            Select = { "id", "category", "title", "content" }
        };

        var response = await _searchClient.SearchAsync<SearchDocument>(null, searchOptions);
        var searchResults = new List<AzureInfo>(3);
        await foreach (var result in response.Value.GetResultsAsync())
        {
            searchResults.Add(new AzureInfo
            {
                Id = result.Document["id"].ToString(),
                Category = result.Document["category"].ToString(),
                Title = result.Document["title"].ToString(),
                Content = result.Document["content"].ToString()
            });
        }

        return searchResults;
    }

    private async Task<ReadOnlyMemory<float>> GetVectorAsync(string query)
    {
        var options = new EmbeddingsOptions
        {
            DeploymentName = DeploymentNameAda,
            Input = { query }
        };
        var response = await _aiClient.GetEmbeddingsAsync(options);
        return response.Value.Data[0].Embedding;
    }
}