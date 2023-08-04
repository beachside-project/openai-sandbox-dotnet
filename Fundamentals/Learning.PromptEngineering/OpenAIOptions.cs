using Microsoft.Extensions.Configuration;

namespace Learning.PromptEngineering;

public class OpenAIOptions
{
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
    public string DeploymentName { get; set; }

    public static OpenAIOptions ReadFromUserSecrets()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();
        return builder.Build().GetRequiredSection(nameof(OpenAIOptions)).Get<OpenAIOptions>();
    }
}