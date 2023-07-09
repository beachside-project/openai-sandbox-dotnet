using Microsoft.Extensions.Configuration;

namespace ConsoleApp1;

public class AppOptions
{
    public string CognitiveSearchEndpoint { get; set; }
    public string CognitiveSearchAdminKey { get; set; }
    public string CognitiveSearchIndexName { get; set; }
    public string OpenAIKey { get; set; }
    public string OpenAIEndpoint { get; set; }
    public string  OpenAIEmbeddingsDeploymentName { get; set; }

    public static AppOptions ReadAppConfigFromUserSecrets()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();
        return builder.Build().GetSection(nameof(AppOptions)).Get<AppOptions>();
    }
}