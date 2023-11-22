using Azure.AI.OpenAI;
using Azure;

namespace AISearchSamples;

internal class Program
{
    static async Task Main()
    {
        var appOptions = AppOptions.ReadAppConfigFromUserSecrets();
        var client = new OpenAIClient(new Uri(appOptions.OpenAiEndpoint), new AzureKeyCredential(appOptions.OpenAiKey));

        var sample02 = new Sample02Embeddings();
        await sample02.RunAsync(client, appOptions);


    }
}
