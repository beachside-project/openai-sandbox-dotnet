using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core;

namespace OpenAISdkSamples;

internal class ChatCompletionSample1
{
    public static async Task RunAsync(OpenAIOptions config)
    {
        var client = CreateClientWithCustomizedRetryOption(config);
        //await RunChatCompletionsAsync(config, client);
        await RunChatStreamingAsync(config, client);
    }

    private static OpenAIClient CreateClientWithCustomizedRetryOption(OpenAIOptions config)
    {
        var options = new OpenAIClientOptions
        {
            Retry =
            {
                Delay = TimeSpan.FromSeconds(2),
                MaxRetries = 10,
                Mode = RetryMode.Fixed
            }
        };

        return new OpenAIClient(new Uri(config.Endpoint), new AzureKeyCredential(config.ApiKey), options);
    }

    private static async Task RunChatCompletionsAsync(OpenAIOptions config, OpenAIClient client)
    {
        var options = new ChatCompletionsOptions
        {
            DeploymentName = config.DeploymentName,
            MaxTokens = 1000,
            Messages =
            {
                new ChatRequestSystemMessage("""
                                あなたは Azure の専門家です。初心者にやさしい口調で回答を返します。
                                """)
            }
        };

        var userMessages = new[]
        {
            "こんにちは、私はあつしです。",
            "Azure Open AI とはなんですか。",
            "私の名前を覚えていますか。"
        };

        foreach (var userMessage in userMessages)
        {
            options.Messages.Add(new ChatRequestUserMessage(userMessage));
            Console.WriteLine($"{ChatRole.User}: {userMessage}");

            var response = await client.GetChatCompletionsAsync(options);

            foreach (var choice in response.Value.Choices)
            {
                Console.WriteLine($"{choice.Message.Role}: {choice.Message.Content}");
                options.Messages.Add(new ChatRequestAssistantMessage(choice.Message.Content));
            }

            // CompletionTokens: 今回応答したメッセージのトークン数。
            // PromptTokens は、今回の CompletionTokens (ようは今回のレスポンスのメッセージ) 以外の prompt のトークン数。
            Console.WriteLine($"TotalTokens: {response.Value.Usage.TotalTokens} (CompletionTokens: {response.Value.Usage.CompletionTokens}; PromptTokens: {response.Value.Usage.PromptTokens})");
        }
    }

    private static async Task RunChatStreamingAsync(OpenAIOptions config, OpenAIClient client)
    {
        var options = new ChatCompletionsOptions
        {
            DeploymentName = config.DeploymentName,
            MaxTokens = 1000,
            Messages =
            {
                new ChatRequestSystemMessage("""
        あなたは Azure の専門家です。初心者にやさしい口調で回答を返します。
        """)
            }
        };

        var userMessages = new[]
        {
            "こんにちは、私はあつしです。",
            "Azure Open AI とはなんですか。100文字くらいで説明してください",
            "私の名前を覚えていますか。"
        };

        foreach (var userMessage in userMessages)
        {
            options.Messages.Add(new ChatRequestUserMessage(userMessage));
            var assistantMessage = string.Empty;
            
            await foreach (StreamingChatCompletionsUpdate chatUpdate in await client.GetChatCompletionsStreamingAsync(options))
            {
                if (chatUpdate.Role.HasValue)
                {
                    Console.Write($"{chatUpdate.Role.Value}: ");
                }
                if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
                {
                    Console.Write(chatUpdate.ContentUpdate);
                    assistantMessage += chatUpdate.ContentUpdate;
                }
            }

            options.Messages.Add(new ChatRequestAssistantMessage(assistantMessage));

            Console.WriteLine("\n----------------------------------------------\n");
        }

        Console.WriteLine($"Messages count: {options.Messages.Count}");
        Console.WriteLine("\n^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n");
    }
}