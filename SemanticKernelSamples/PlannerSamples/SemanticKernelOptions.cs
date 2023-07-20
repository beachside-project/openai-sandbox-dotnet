using Microsoft.Extensions.Configuration;

namespace PlannerSamples;

internal class SemanticKernelOptions
{
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
    public string DeploymentNameForChatCompletion { get; set; }
    public string DeploymentNameForEmbeddings { get; set; }
    public string CognitiveSearchEndpoint { get; set; }
    public string CognitiveSearchApiKey { get; set; }
    public string BingWebSearchKey { get; set; }

    public static SemanticKernelOptions CreateClientFromUserSecrets()
        => new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build()
            .GetSection(nameof(SemanticKernelOptions)).Get<SemanticKernelOptions>();
}
