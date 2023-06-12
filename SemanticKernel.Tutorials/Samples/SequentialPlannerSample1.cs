using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.SemanticFunctions;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace SemanticKernel.Tutorials.Samples;

internal class SequentialPlannerSample1
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        var kernel = new KernelBuilder()
            // TextCompletion だと token 数少ないので注意
            //.WithAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey)
            .WithAzureChatCompletionService(options.DeploymentNameForChatCompletion, options.Endpoint, options.ApiKey)
            .Build();

        kernel.CreateSemanticFunction("""
            あなたはテキストのライターです。
            与えられた文章に対して "タイトル" を付けて、"序文" と "まとめ" の文章を作成してください。

            出力例:
            ```
            # タイトルを記載

            ## 序文

            ここに序文を記載してください。

            ## 4つのトピック

            {{$input}}

            ## まとめ

            入力の要約をここに書いてください。
            ```

            文章:
            ```
            {{$input}}
            ```
            """,
            config: new PromptTemplateConfig
            {
                Input = new PromptTemplateConfig.InputConfig
                {
                    Parameters =
                    {
                        new() { Name = "input", Description = "文章"}
                    },
                },
                Description = "文章に対してタイトルを付けて、まとめの章と序文を追加してください。",
                Completion = new PromptTemplateConfig.CompletionConfig
                {
                    MaxTokens = 5000,
                },
            },
            "Summarize",
            "WriterSkill");

        kernel.CreateSemanticFunction("""
            与えられたカンマ区切りのキーワードを使って文章を作成してください。
            キーワードが 4 つの場合には、4つのセクションを作成してください。
            セクションの前には、そのキーワードを見出しとして記載してください。

            入力が "キーワード1,キーワード2,キーワード3,キーワード4" の場合の出力例は以下のようになります。

            出力例:
            ```
            ### キーワード1

            このキーワードに関する文章を2から3段落で記載します。

            ### キーワード2

            このキーワードに関する文章を2から3段落で記載します。

            ### キーワード3

            このキーワードに関する文章を2から3段落で記載します。

            ### キーワード4

            このキーワードに関する文章を2から3段落で記載します。
            ```

            キーワード: {{$input}}
            """,
            config: new PromptTemplateConfig
            {
                Input = new PromptTemplateConfig.InputConfig
                {
                    Parameters =
                    {
                        new()
                        {
                            Name = "input",
                            Description = "カンマ区切りのキーワード",
                            DefaultValue = null
                        }
                    }
                },
                Description = "キーワードを元に文章を作成します。",
                Completion = new PromptTemplateConfig.CompletionConfig
                {
                    MaxTokens = 2000,
                }
            },
            "Write",
            "WriterSkill");

        kernel.CreateSemanticFunction(""""
            与えられた単語・文章から関連性の強いキーワードを4つ回答してください。
            単語はカンマ区切りで1行で回答してください。

            出力例:
            入力が "リンゴ" の場合の出力例は以下のようになります。
            ```
            ビタミンC,果物,リンゴ酸,青森
            ```

            トピック: {{$input}}
            """",
            config: new PromptTemplateConfig()
            {
                Input = new PromptTemplateConfig.InputConfig()
                {
                    Parameters =
                    {
                        new PromptTemplateConfig.InputParameter()
                        {
                            Name = "input",
                            Description = "トピックを作るための単語・文章"
                        },
                    }
                },
                Description = "入力された単語・文章から4つのキーワードを生成します。",
            },
            "CreateKeywords",
            "IdeaSkill");

        kernel.CreateSemanticFunction("""
            あなたは天才コメディアンです。アイテムに対して面白い返答をしてください。
            アイテム: {{$input}}
            """,
            config: new PromptTemplateConfig()
            {
                Input = new PromptTemplateConfig.InputConfig()
                {
                    Parameters =
                    {
                        new PromptTemplateConfig.InputParameter() { Name = "input", Description = "アイテム" },
                    },
                },
                Description = "アイテムに対して面白い話を返します。",
            },
            "Joke",
            "FunSkill");

        // plan の生成・実行
        var planner = new SequentialPlanner(kernel);

        var keyword = "JR東日本について";
        var plan = await planner.CreatePlanAsync(keyword);

        Console.WriteLine("PLAN:");
        Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) }));

        var needStepRun = true;
        if (needStepRun)
        {
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
        else
        {
            var context = await kernel.RunAsync(plan);
            Console.WriteLine(context);
        }


    }
}