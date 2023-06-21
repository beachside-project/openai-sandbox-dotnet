using Azure.AI.OpenAI;

namespace Learning.PromptEngineering;

internal class _3_Summarizing_Inferring
{
    internal static async Task RunAsync()
    {
        var options = OpenAIOptions.ReadFromUserSecrets();
        var client = Helper.CreateClient(options);

        //var reviewText = """
        //    寝室におしゃれなランプが欲しかったのですが、これは追加の収納スペースもついているのに価格もそれほど高くありませんでした。
        //    発送中にランプのケーブルが切れてしまったようですが、会社はすぐに新しいものを送ってくれました。
        //    こちらも数日以内に届きました。組み立ても簡単でした。
        //    足りない部品があったのでサポートに連絡したところ、こちらもすぐに発送してくれました。
        //    ルミナは顧客と製品を大切にする素晴らしい会社と感じました。
        //    """;

        var reviewText = """
            開封済みの物が届いた。
            誰が使ったかもわからないので気分が悪いです。EPOS から二度と商品を買いません。
            """;

        var prompt = $"""
            Review Text は 3 つのバッククォートで区切られています。
            あなたタスクは、Review Text から次のことを特定します。

            - 感情 ("Positive" または "Negative" のどちらかで回答)
            - レビューアーは、怒りの感情を持っているか ( "true" または "False" で回答)
            - レビューアーが購入した商品。情報が存在しない場合は "Item unknown" と回答。
            - 商品のブランド。情報が存在しない場合は "Brand unknown" と回答。

            回答はできるだけ短くしてください。

            Review Text: ``` {reviewText} ```
            """;

        var result = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(result);
    }

    private static async Task<string> GetChatCompletionAsync(OpenAIClient client, OpenAIOptions options, string prompt)
    {
        var chatCompletionOptions = new ChatCompletionsOptions
        {
            MaxTokens = 4000,
            Messages =
            {
                new ChatMessage(ChatRole.User, prompt)
            }
        };

        var response = await client.GetChatCompletionsAsync(options.DeploymentName, chatCompletionOptions);
        return response.Value.Choices[0].Message.Content;
    }
}