using Microsoft.Extensions.Configuration;

namespace AISearchSamples;

public class AppOptions
{
    public string OpenAiEndpoint { get; set; }
    public string OpenAiKey { get; set; }
    public string AdaDeploymentName { get; set; }
    public string GptDeploymentName { get; set; }
    public string SearchEndpoint { get; set; }
    public string SearchKey { get; set; }
    public string SearchIndexName { get; set; }

    public static AppOptions ReadAppConfigFromUserSecrets()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();
        return builder.Build().GetSection(nameof(AppOptions)).Get<AppOptions>()?? throw new NullReferenceException("AppOptions in user secrets");
    }
}