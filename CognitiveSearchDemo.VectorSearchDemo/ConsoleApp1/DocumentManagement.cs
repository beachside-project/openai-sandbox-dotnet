using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using System.Text.Json;

namespace ConsoleApp1;

internal class DocumentManagement
{
    internal async Task RunAsync(AppOptions options)
    {
        await AddDocumentsAsync(options);
    }

    private async Task AddDocumentsAsync(AppOptions options)
    {
        var openAIClient = new OpenAIClient(new Uri(options.OpenAIEndpoint), new AzureKeyCredential(options.OpenAIKey));
        var searchClient = new SearchClient(new Uri(options.CognitiveSearchEndpoint), options.CognitiveSearchIndexName, new AzureKeyCredential(options.CognitiveSearchAdminKey));

        const string FilePath = @"C:\repos\azure-openai\openai-sandbox-dotnet\CognitiveSearchDemo.VectorSearchDemo\ConsoleApp1\sample-data.json";
        var json = await File.ReadAllTextAsync(FilePath);

        // record 使うのとどっちがいいかは迷う。dic だと型意識せずに対応ができるが怖いねぇ。
        //var sampleDataList = JsonSerializer.Deserialize<List<SampleDocument>>(json);

        // SearchDocument 型の constructor が dictionary でイケるため
        var sampleDictionaryList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);

        var documentsToUpload = new List<SearchDocument>();

        foreach (var dic in sampleDictionaryList)
        {
            var contentVector = (await GenerateEmbeddingsAsync(openAIClient, options, dic["content"].ToString())).ToArray();
            dic["contentVector"] = contentVector;

            var titleVector = (await GenerateEmbeddingsAsync(openAIClient, options, dic["title"].ToString())).ToArray();
            dic["titleVector"] = titleVector;

            documentsToUpload.Add(new SearchDocument(dic));
        }

        var res = await searchClient.IndexDocumentsAsync(IndexDocumentsBatch.MergeOrUpload(documentsToUpload));
    }

    private async Task<IReadOnlyList<float>> GenerateEmbeddingsAsync(OpenAIClient client, AppOptions options, string inputText)
    {
        var response = await client.GetEmbeddingsAsync(options.OpenAIEmbeddingsDeploymentName, new EmbeddingsOptions(inputText));
        return response.Value.Data[0].Embedding;
    }
}

internal record SampleDocument(string id, string title, string content, string category);