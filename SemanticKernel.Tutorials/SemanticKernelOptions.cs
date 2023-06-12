using Microsoft.Extensions.Configuration;

namespace SemanticKernel.Tutorials;

public class SemanticKernelOptions
{
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
    public string DeploymentNameForTextCompletion { get; set; }
    public string DeploymentNameForChatCompletion { get; set; }
    public string DeploymentNameForEmbeddings { get; set; }

    public static SemanticKernelOptions CreateClientFromUserSecrets()
    {
        return new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build()
            .GetSection(nameof(SemanticKernelOptions)).Get<SemanticKernelOptions>();
    }
}