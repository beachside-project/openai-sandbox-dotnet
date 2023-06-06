using Azure;
using Azure.AI.OpenAI;

namespace OpenAISdkSamples;

internal class ChatCompletionSample1
{
    public static async Task RunAsync(OpenAiConfig config)
    {
        var client = new OpenAIClient(new Uri(config.Endpoint), new AzureKeyCredential(config.ApiKey));
        await GetChatCompletionsAsync(config, client);
        //await GetChatCompletionsStreamingAsync(config, client);


    }

    private static async Task GetChatCompletionsAsync(OpenAiConfig config, OpenAIClient client)
    {
        var options = new ChatCompletionsOptions
        {
            MaxTokens = 300,
            Messages =
            {
                new ChatMessage(ChatRole.System, """
        あなたは Azure の専門家です。250文字以内で回答を返します。
        """)
            }
        };

        var firstResponse = await client.GetChatCompletionsAsync(config.ModelName, options);

        var userMessages = new[]
        {
            "こんにちは、私はあつしです。",
            "Azure Open AI とはなんですか。",
            "私の名前を覚えていますか。"
        };

        foreach (var userMessage in userMessages)
        {
            options.Messages.Add(new ChatMessage(ChatRole.User, userMessage));
            Console.WriteLine($"{ChatRole.User}: {userMessage}");

            var response = await client.GetChatCompletionsAsync(config.ModelName, options);

            foreach (var choice in response.Value.Choices)
            {
                Console.WriteLine($"{choice.Message.Role}: {choice.Message.Content}");
                options.Messages.Add(new ChatMessage(choice.Message.Role, choice.Message.Content));
            }

            // CompletionTokens: 今回応答したメッセージのトークン数。
            // PromptTokens は、今回の CompletionTokens (ようは今回のレスポンスのメッセージ) 以外の prompt のトークン数。
            Console.WriteLine($"TotalTokens: {response.Value.Usage.TotalTokens} (CompletionTokens: {response.Value.Usage.CompletionTokens}; PromptTokens: {response.Value.Usage.PromptTokens})");
        }
    }

    private static async Task GetChatCompletionsStreamingAsync(OpenAiConfig config, OpenAIClient client)
    {
        var options = new ChatCompletionsOptions
        {
            MaxTokens = 300,
            Messages =
            {
                new ChatMessage(ChatRole.System, """
        あなたは Azure の専門家です。250文字以内で回答を返します。
        """)
            }
        };

        var firstResponse = await client.GetChatCompletionsStreamingAsync(config.ModelName, options);

        var userMessages = new[]
        {
            "こんにちは、私はあつしです。",
            "Azure Open AI とはなんですか。50文字くらいで説明してください",
            "私の名前を覚えていますか。"
        };

        foreach (var userMessage in userMessages)
        {
            options.Messages.Add(new ChatMessage(ChatRole.User, userMessage));
            Console.WriteLine($"{ChatRole.User}: {userMessage}");

            var response = await client.GetChatCompletionsStreamingAsync(config.ModelName, options);

            using var streamingChatCompletions = response.Value;
            Console.Write("assistant: ");
            await foreach (var choice in streamingChatCompletions.GetChoicesStreaming())
            {
                await foreach (var message in choice.GetMessageStreaming())
                {
                    // 1トークンづつとれる (たぶん。正確なトークンかは不明)
                    Console.Write(message.Content);
                }
                Console.WriteLine();
            }
        }

    }
}