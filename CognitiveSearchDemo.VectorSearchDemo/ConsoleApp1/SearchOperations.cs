using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace ConsoleApp1;

internal class SearchOperations
{
    internal async Task RunAsync(AppOptions options)
    {
        var openAIClient = new OpenAIClient(new Uri(options.OpenAIEndpoint), new AzureKeyCredential(options.OpenAIKey));
        var searchClient = new SearchClient(new Uri(options.CognitiveSearchEndpoint), options.CognitiveSearchIndexName, new AzureKeyCredential(options.CognitiveSearchAdminKey));

        const string vectorField = "titleVector";
        const string query = "security feature";
        await SearchWithSingleVectorAsync(searchClient, openAIClient, options, vectorField, query);
    }

    private async Task SearchWithSingleVectorAsync(SearchClient searchClient, OpenAIClient openAIClient, AppOptions options, string vectorField, string query)
    {
        const int dataCount = 3;

        if (vectorField != "contentVector" && vectorField != "titleVector")
        {
            Console.WriteLine($"Target vector field is invalid (input: {vectorField})");
            return;
        }

        var queryEmbeddings = await GenerateEmbeddingsAsync(openAIClient, options, query);

        var searchQueryVector = new SearchQueryVector
        {
            // K-NN で予測するデータから近い順に取得するデータの個数
            KNearestNeighborsCount = dataCount,
            Fields = vectorField,
            Value = queryEmbeddings.ToArray()
        };
        var searchOptions = new SearchOptions
        {
            Vector = searchQueryVector,
            Size = dataCount,
            Select = { "title", "content", "category" }
        };

        SearchResults<SearchDocument> response = await searchClient.SearchAsync<SearchDocument>(null, searchOptions);

        await foreach (var result in response.GetResultsAsync())
        {
            Console.WriteLine($"Title: {result.Document["title"]}");
            Console.WriteLine($"Score: {result.Score}");
            Console.WriteLine($"Content: {result.Document["content"]}");
            Console.WriteLine($"Category: {result.Document["category"]}");
            Console.WriteLine();
        }
    }

    private async Task SearchWithVectorAndFilterAsync(SearchClient searchClient, OpenAIClient openAIClient, AppOptions options, string vectorField, string query, string filter)
    {
        //https://github.com/Azure/cognitive-search-vector-pr/blob/718183770e2df29c55e65d339f4d5110dd7e9441/demo-dotnet/code/Program.cs#L126
    }

    private async Task<IReadOnlyList<float>> GenerateEmbeddingsAsync(OpenAIClient client, AppOptions options, string inputText)
    {
        var response = await client.GetEmbeddingsAsync(options.OpenAIEmbeddingsDeploymentName, new EmbeddingsOptions(inputText));
        return response.Value.Data[0].Embedding;
    }
}