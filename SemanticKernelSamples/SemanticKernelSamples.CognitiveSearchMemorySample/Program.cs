using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.AzureSearch;

namespace SemanticKernelSamples.CognitiveSearchMemorySample;

internal class Program
{
    private static async Task Main()
    {
        var options = SemanticKernelOptions.CreateClientFromUserSecrets();

        var kernel = Kernel.Builder
            .WithAzureTextEmbeddingGenerationService(options.DeploymentNameForEmbeddings, options.Endpoint, options.ApiKey)
            .WithMemoryStorage(new AzureSearchMemoryStore(options.CognitiveSearchEndpoint, options.CognitiveSearchApiKey))
            .Build();

        //await GitHubReadmeDataSample.RunAsync(kernel);
        await ComicSearchSample.RunAsync(kernel);
    }
}