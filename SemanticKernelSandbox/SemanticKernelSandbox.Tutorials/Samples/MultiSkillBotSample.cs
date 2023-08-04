using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.SemanticFunctions;
using SemanticKernel.Tutorials.Samples.Skills;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace SemanticKernel.Tutorials.Samples;

internal class MultiSkillBotSample
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        var kernel = new KernelBuilder()
            // TextCompletion だと token 数少ないので注意
            //.WithAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey)
            .WithAzureChatCompletionService(options.DeploymentNameForChatCompletion, options.Endpoint, options.ApiKey)
            .Build();

        kernel.CreateSemanticFunction(@"
            あなたは天才コメディアンです。ジョークを作ってほしい、面白い話しをしてほしいというニュアンスの質問に対して、input から面白い返答をしてください。

            input: {{$input}}
            ",
            config: new PromptTemplateConfig()
            {
                Input = new PromptTemplateConfig.InputConfig()
                {
                    Parameters =
                    {
                        new PromptTemplateConfig.InputParameter() { Name = "input", Description = "アイテム" },
                    },
                },
                Description = "input に対して面白い話を返します。",
            },
            "Joke",
            "FunSkill");

        kernel.AddStoryWriterSkillSet();
        kernel.AddLockerSearchSkillSet();

        // plan の生成・実行
        var planner = new SequentialPlanner(kernel);

        //var sentence = "「リンゴ」について文章を作成してください";
        //var sentence = "マイクロソフトについてのジョークを話して";
        var sentence = "東京駅のロッカーを検索して";

        var plan = await planner.CreatePlanAsync(sentence);

        KernelHelper.LogPlan(plan);


        await KernelHelper.RunAsync(kernel, plan, needStepRun: true);
    }
}