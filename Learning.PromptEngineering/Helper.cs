using Azure;
using Azure.AI.OpenAI;

namespace Learning.PromptEngineering;

public class Helper
{
    public static OpenAIClient CreateClient(OpenAIOptions options) => new (new Uri(options.Endpoint), new AzureKeyCredential(options.ApiKey));
}