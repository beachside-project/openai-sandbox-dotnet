﻿using Microsoft.Extensions.Configuration;

namespace OpenAISdkSamples;

// 実行前に UserSecrets か local.settings.json で以下を構成して値をセットする。
// {
//     "OpenAIConfig": {
//         "Endpoint": "",
//         "ApiKey": "",
//         "DeploymentName": ""
//     }
// }

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

    public static OpenAIOptions ReadFromLocalSettingsJson()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json");
        return builder.Build().GetRequiredSection(nameof(OpenAIOptions)).Get<OpenAIOptions>();
    }
}