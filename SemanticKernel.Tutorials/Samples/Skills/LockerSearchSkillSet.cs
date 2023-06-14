using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace SemanticKernel.Tutorials.Samples.Skills;

public static class LockerSearchSkillSetExtension
{
    public static IKernel AddLockerSearchSkillSet(this IKernel kernel)
    {
        var lockerApiSkill = kernel.ImportSkill(new LockerSearchNativeFunction(), nameof(LockerSearchNativeFunction));
        // 単独実行の検証するならこんな感じ。
        //var context = await kernel.RunAsync("tokyo", lockerApiSkill["SearchLocker"]);
        // [{"key":"111","value":"value111"},{"key":"222","value":"value222"},{"key":"333","value":"value333"}]

        kernel.CreateSemanticFunction("""
            あなたは Baggage locker search のチャットボットです。
            ロッカーを探しているユーザーに回答します。
            与えられたロッカーの場所は JSON で入力されます。それを箇条書きにして回答してください。回答例は以下です。

            回答例:
            ```
            ロッカーの場所: [{"key":"aaa","value":"bbb"},{"key":"ccc","value":"ddd"}
            回答:
            ロッカーの検索結果は以下です。

            - aaa: bbb
            - ccc: ddd
            ```

            ロッカーの場所: {{$input}}
            回答:

            """,
             config: new PromptTemplateConfig
             {
                 Input = new PromptTemplateConfig.InputConfig
                 {
                     Parameters =
                     {
                                new() { Name = "input", Description = "Baggage locker search の結果の JSON" }
                     },
                 },
                 Description = "Baggage locker を探しているユーザーにBaggage locker search の結果を回答をします。",
                 Completion = new PromptTemplateConfig.CompletionConfig
                 {
                     MaxTokens = 5000,
                 },
             },
             "Summarize",
             "LockerSearchSkill");

        kernel.CreateSemanticFunction("""
            あなたは Baggage locker search のチャットボットです。
            Baggage locker を探している質問の場合、質問から Baggage locker の場所のキーワードを抽出します。
            出力は JSON 形式にします。
            j回答例は以下です。

            キーワードの抽出例1:
            ```
            質問: 東京のロッカーの場所を教えて
            回答: {"location": "東京"}
            ```

            キーワードの抽出例2:
            ```
            質問: 福岡駅にロッカーはある？
            回答: {"location": "福岡駅"}
            ```

            キーワードの抽出例3:
            ```
            質問: 長野駅のロッカーで315番は空いてる？
            回答: {"location": "長野駅", "locker": "315"}
            ```

            キーワードの抽出例3:
            ```
            質問: 横浜駅のロッカーでNo.50は空いてる？
            回答: {"location": "横浜駅", "locker": "50"}
            ```

            質問: {{$input}}
            回答:
            """,
            config: new PromptTemplateConfig
            {
                Input = new PromptTemplateConfig.InputConfig
                {
                    Parameters =
                    {
                        new() { Name = "input", Description = "質問" }
                    },
                },
                Description = "ロッカーを探している質問の場合、ロッカーの場所のキーワードを抽出します。",
                Completion = new PromptTemplateConfig.CompletionConfig
                {
                    MaxTokens = 1000,
                },
            },
            "Keyword",
            "LockerSearchSkill");

        return kernel;
    }
}