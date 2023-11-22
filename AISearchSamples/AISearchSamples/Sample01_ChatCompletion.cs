using Azure.AI.OpenAI;

namespace AISearchSamples;

public class Sample01ChatCompletion
{
    public async Task RunAsync(OpenAIClient client, AppOptions appOptions)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            DeploymentName = appOptions.GptDeploymentName,
            Messages =
            {
                new ChatMessage(ChatRole.System, "あなたは陽気なアシスタントチャットボットです。"),
                new ChatMessage(ChatRole.User, "こんにちは")
            }
        };

        var response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
        var assistantMessage = response.Value.Choices.First().Message.Content;

        Console.WriteLine(assistantMessage);
    }
}