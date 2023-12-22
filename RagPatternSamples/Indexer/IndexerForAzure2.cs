using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Indexer;

public class IndexerForAzure2
{
    private static readonly string DeploymentNameAda = "text-embedding-ada-002";

    private readonly OpenAIClient _aiClient;
    private readonly SearchClient _searchClient;

    public IndexerForAzure2(OpenAIClient aiClient, SearchClient searchClient)
    {
        _aiClient = aiClient;
        _searchClient = searchClient;
    }

    [FunctionName("IndexerForAzure2")]
    public async Task Run([CosmosDBTrigger(
            databaseName: "aoai",
            containerName: "azure",
            Connection = "CosmosConnection",
            LeaseContainerName = "leases",
            LeaseContainerPrefix = "azure2")]IReadOnlyList<AzureInfo> items,
        ILogger log)
    {
        log.LogInformation($"IndexerForAzure2 triggered !");

        var documentsToUpload = new List<Azure2Doc>();

        foreach (var item in items)
        {
            var (titleVector, contentVector) = await GetVectorAsync(item.Title, item.Content);

            documentsToUpload.Add(new Azure2Doc
            {
                Id = item.Id,
                Category = item.Category,
                Title = item.Title,
                Content = item.Content,
                TitleVector = titleVector,
                ContentVector = contentVector
            });
        }

        await _searchClient.MergeOrUploadDocumentsAsync(documentsToUpload);
        log.LogInformation($"{items.Count} document(s) uploaded");
    }

    private async Task<(IReadOnlyList<float> titleVector, IReadOnlyList<float> contentVector)> GetVectorAsync(string title, string content)
    {
        var options = new EmbeddingsOptions
        {
            DeploymentName = DeploymentNameAda,
            Input = { title, content }
        };

        var response = await _aiClient.GetEmbeddingsAsync(options);
        var titleVector = response.Value.Data[0].Embedding.ToArray();
        var contentVector = response.Value.Data[1].Embedding.ToArray();
        return (titleVector, contentVector);
    }
}