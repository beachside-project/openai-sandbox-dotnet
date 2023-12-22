using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Indexer;

public class SimpleChat
{
    private static readonly string DeploymentNameAda = "text-embedding-ada-002";
    private static readonly string DeploymentNameGpt = "gpt-4";

    private readonly OpenAIClient _aiClient;
    private readonly SearchClient _searchClient;

    public SimpleChat(OpenAIClient aiClient, SearchClient searchClient)
    {
        _aiClient = aiClient;
        _searchClient = searchClient;
    }

    [FunctionName("ChatWithAoai")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "chat/simple")] HttpRequest req,
        ILogger log)
    {
        var query = req.Query["query"];

        if (string.IsNullOrWhiteSpace(query))
        {
            return new BadRequestObjectResult("Please pass a 'query' on the query string or in the request body");
        }

        var references = await GetReferencesAsync(query);
        var answer = await GenerateAnswerAsync(query, references);

        return new OkObjectResult(answer);
    }

    private async Task<List<AzureInfo>> GetReferencesAsync(string query)
    {
        var queryVector = await GetVectorAsync(query);

        var vectorizedQuery = new VectorizedQuery(queryVector)
        {
            KNearestNeighborsCount = 3,
            Fields = { "contentVector" }
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
                Id = result.Document["id"] as string,
                Category = result.Document["category"] as string,
                Title = result.Document["title"] as string,
                Content = result.Document["content"] as string
            });
            Console.WriteLine(result.Score);
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

    private async Task<string> GenerateAnswerAsync(string query, List<AzureInfo> references)
    {
        var options = new ChatCompletionsOptions
        {
            DeploymentName = DeploymentNameGpt,
            Temperature = 0,
        };

        var referencesJson = JsonSerializer.Serialize(references);
        var systemMessage = new ChatRequestSystemMessage($@"
あなたは Azure のサービスを紹介する専門家です。ユーザーからの質問に対して、以下の references tag の情報を元に回答してください。
<references>
{referencesJson}
</references>

回答は、サービス名とその説明を含めて簡潔にまとめてください。妥当な回答がみつけれない場合はわかりませんと返答してください。
Microsoft Azure に関する質問以外には「Microsoft Azure に関連しない質問にはお答えできません。」と回答してください。");

        var userMessage = new ChatRequestUserMessage(query);

        options.Messages.Add(systemMessage);
        options.Messages.Add(userMessage);

        var response = await _aiClient.GetChatCompletionsAsync(options);
        return response.Value.Choices[0].Message.Content;
    }
}