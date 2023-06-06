using Microsoft.Extensions.Configuration;

namespace OpenAISdkSamples;

public class OpenAiConfig
{
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
    public string ModelName { get; set; }

    public static OpenAiConfig ReadFromUserSecrets()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();
        return builder.Build().GetSection("OpenAIConfig").Get<OpenAiConfig>();
    }
}