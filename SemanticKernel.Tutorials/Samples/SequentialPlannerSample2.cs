using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using SemanticKernel.Tutorials.Samples.Skills;

namespace SemanticKernel.Tutorials.Samples;

internal class SequentialPlannerSample2
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        var kernel = new KernelBuilder()
            .WithAzureChatCompletionService(options.DeploymentNameForChatCompletion, options.Endpoint, options.ApiKey)
            .Build();

        var lockerApiSkill = kernel.ImportSkill(new LockerSearchSkill(), nameof(LockerSearchSkill));

        //var context = await kernel.RunAsync("tokyo", lockerApiSkill["SearchLocker"]);
        // [{"key":"111","value":"value111"},{"key":"222","value":"value222"},{"key":"333","value":"value333"}]


        kernel.CreateSemanticFunction("""
            あなたは Baggage locker search のチャットボットです。
            ロッカーを探しているユーザーに回答します。
            ロッカーを探している質問以外には、「私はロッカー検索ボットです。他のトピックに関する回答はできません。」と回答します。
            与えられたロッカーの場所は JSON で入力されます。それを箇条書きにして回答してください。

            回答例:
            ```
            入力: [{"key":"aaa","value":"bbb"},{"key":"ccc","value":"ddd"}
            回答: 
            ロッカーの検索結果は以下です。

            - aaa: bbb
            - ccc: ddd
            ```

            入力: {{$input}}
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
            "WriterSkill");

        kernel.CreateSemanticFunction("""
            Baggage locker を探している質問の場合、入力の文章から Baggage locker の場所のキーワードを抽出します。
            出力は JSON 形式にします。

            キーワードの抽出例1:
            ```
            入力: 東京のロッカーの場所を教えて
            回答: {"location": "東京"}            
            ```

            キーワードの抽出例2:
            ```
            入力: 福岡駅にロッカーはある？
            回答: {"location": "福岡駅"}            
            ```

            キーワードの抽出例3:
            ```
            入力: 長野駅のロッカーで315番は空いてる？
            回答: {"location": "長野駅", "locker": "315"}            
            ```

            キーワードの抽出例3:
            ```
            入力: 横浜駅のロッカーでNo.50は空いてる？
            回答: {"location": "横浜駅", "locker": "50"}            
            ```

            入力: {{$input}}
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
            "LockerSkill");

        var planner = new SequentialPlanner(kernel);

        var question = "札幌駅で50番のロッカーは空いてるか";
        var plan = await planner.CreatePlanAsync(question);

        Console.WriteLine("PLAN:");
        Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) }));


        while (plan.HasNextStep)
        {
            Console.WriteLine($"""
                * PLAN: {plan.Steps[plan.NextStepIndex].Name} の実行結果:
                """);
            await kernel.StepAsync(plan);
            Console.WriteLine($"""
                {plan.State.Input}

                """);
        }
    }
}