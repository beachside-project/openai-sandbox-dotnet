using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Indexer;

public class ChatWithSemanticKernel
{
    //private static readonly string DeploymentNameAda = "text-embedding-ada-002";
    //private static readonly string DeploymentNameGpt = "gpt-4";

    //private readonly OpenAIClient _aiClient;
    //private readonly SearchClient _searchClient;

    //public ChatWithSemanticKernel(OpenAIClient aiClient, SearchClient searchClient)
    //{
    //    _aiClient = aiClient;
    //    _searchClient = searchClient;
    //}

    //[FunctionName("ChatWithSemanticKernel")]
    //public async Task<IActionResult> Run(
    //    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "chat/sk")] HttpRequest req,
    //    ILogger log)
    //{
    //    var query = req.Query["query"];

    //    if (string.IsNullOrWhiteSpace(query))
    //    {
    //        return new BadRequestObjectResult("Please pass a 'query' on the query string or in the request body");
    //    }

    //    var references = await GetReferencesAsync(query);


    //    return new OkObjectResult("responseMessage");
    //}

    //private async Task<List<AzureInfo>> GetReferencesAsync(string query)
    //{
    //    var queryVector = await GetVectorAsync(query);
    //    var references = await GetReferencesAsync(queryVector);

    //}

    //private async Task<ReadOnlyMemory<float>> GetVectorAsync(string query)
    //{
    //    var options = new EmbeddingsOptions
    //    {
    //        DeploymentName = DeploymentNameAda,
    //        Input = { query }
    //    };
    //    var response = await _aiClient.GetEmbeddingsAsync(options);
    //    return response.Value.Data[0].Embedding;
    //}
}