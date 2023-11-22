using System.Text.Json;
using Azure.AI.OpenAI;

namespace AISearchSamples;

internal class Sample02Embeddings
{
    internal async Task RunAsync(OpenAIClient client, AppOptions appOptions)
    {
        var embeddingsOptions = new EmbeddingsOptions
        {
            DeploymentName = appOptions.AdaDeploymentName,
            Input =
            {
                "こんにちは",
                "ハロー"
            }
        };

        var response = await client.GetEmbeddingsAsync(embeddingsOptions);

        foreach (var data in response.Value.Data)
        {
            Console.WriteLine($"Index: {data.Index}");
            Console.WriteLine($"{JsonSerializer.Serialize(data.Embedding)}");
            Console.WriteLine();
        }
    }
}