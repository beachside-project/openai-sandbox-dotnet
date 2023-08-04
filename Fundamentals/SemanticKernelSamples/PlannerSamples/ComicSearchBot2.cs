using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Skills.Web;
using Microsoft.SemanticKernel.Skills.Web.Bing;
using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace PlannerSamples;

internal class ComicSearchBot2
{
    internal async Task RunAsync()
    {
        var options = SemanticKernelOptions.CreateClientFromUserSecrets();

        // 全然だめ
        //await RunSequentialPlannerAsync(options);


        //
        await RunCustomPlanAsync(options);
    }

    private async Task RunSequentialPlannerAsync(SemanticKernelOptions options)
    {
        var kernel = Kernel.Builder
            .WithAzureChatCompletionService(options.DeploymentNameForChatCompletion, options.Endpoint, options.ApiKey)
            .WithAzureTextEmbeddingGenerationService(options.DeploymentNameForEmbeddings, options.Endpoint, options.ApiKey)
            .Build();

        // Bing Connector
        // - NuGet: Microsoft.SemanticKernel.Skills.Web
        kernel.ImportSkill(new WebSearchEngineSkill(new BingConnector(options.BingWebSearchKey)), "bing");

        var skPrompt = kernel.CreateSemanticFunction(
            """
            ユーザーは講談社のマンガのタイトルを探しています。
            あなたのタスクは、ユーザーの質問から講談社のマンガのタイトルを検索して回答することです。
            Bing Search API を利用して情報を検索してください。
            複数の候補がある場合は複数のマンガのタイトルを回答してください。
            Bing Search API で取得した情報以外は回答に含めてはいけません。
            Bing Search API の情報から回答が見つからない場合は「わかりません。もう少し情報を教えてください」返答してください。

            回答:
            """
        //, maxTokens: 4000
        );

        // variables の使い方わからん？！？！
        // TODO:
        var question = "講談社 マンガ 家が全焼する炎上事件から村田杏子が御手洗家に潜入して復讐をする話のマンガのタイトルが知りたい";

        // ----------------------
        // SequentialPlanner の利用
        // ----------------------
        var planner = new SequentialPlanner(kernel);
        var plan1 = await planner.CreatePlanAsync(question);
        Console.WriteLine($"""
            plan1:
            {JsonSerializer.Serialize(plan1, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) })}
            """);
        // 実行
        var plan1Result = await plan1.InvokeAsync();
        Console.WriteLine($"""
            Original plan result:
            {WordWrap(plan1Result.Result, 100)}
        """);

    }


    private async Task RunCustomPlanAsync(SemanticKernelOptions options)
    {
        //var question = "講談社 マンガ 家が全焼する炎上事件から村田杏子が御手洗家に潜入して復讐をする話のマンガのタイトルが知りたい";
        var question = "集英社 マンガ ゴム人間が海賊王になるマンガ";

        var kernel = Kernel.Builder
            .WithAzureChatCompletionService(options.DeploymentNameForChatCompletion, options.Endpoint, options.ApiKey)
            .WithAzureTextEmbeddingGenerationService(options.DeploymentNameForEmbeddings, options.Endpoint, options.ApiKey)
            .Build();

        // Bing Connector
        // - NuGet: Microsoft.SemanticKernel.Skills.Web
        kernel.ImportSkill(new WebSearchEngineSkill(new BingConnector(options.BingWebSearchKey)), "bing");

        var skPrompt = kernel.CreateSemanticFunction(
            """
            ユーザーはマンガのタイトルを探しています。
            あなたのタスクは、以下の "REFERENCES" tag の中の情報からマンガのタイトルを回答することです。
            複数の候補がある場合は複数のマンガのタイトルを回答してください。
            REFERENCES タグ内の情報から回答が見つからない場合は「わかりません。もう少し情報を教えてください」返答してください。

            <REFERENCES>
            {{$reference}}
            </REFERENCES>

            回答:
            """,
            skillName: "summarySkill",
            functionName: "summaryFunction"
        );

        var plan1 = new Plan(kernel.Func("bing", "search"))
        {
            Parameters = new ContextVariables
            {
                ["query"] = question,
                [WebSearchEngineSkill.CountParam] = "10"
            },
            Outputs = { "searchResult" }
        };

        var plan2 = new Plan(kernel.Func("summarySkill", "summaryFunction"))
        {
            Parameters = new ContextVariables
            {
                ["reference"] = "$searchResult"
            }
        };

        var executePlan = new Plan(question, plan1, plan2);
        var skContext = await executePlan.InvokeAsync();
        Console.WriteLine(skContext.Result);
        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        Console.WriteLine($"""
            skContext:
            {JsonSerializer.Serialize(skContext.Variables, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) })}
            """);



    }

    private static string WordWrap(string text, int maxLineLength)
    {
        var result = new StringBuilder();
        int i;
        var last = 0;
        var space = new[] { ' ', '\r', '\n', '\t' };
        do
        {
            i = last + maxLineLength > text.Length
                ? text.Length
                : (text.LastIndexOfAny(new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' }, Math.Min(text.Length - 1, last + maxLineLength)) + 1);
            if (i <= last) i = Math.Min(last + maxLineLength, text.Length);
            result.AppendLine(text.Substring(last, i - last).Trim(space));
            last = i;
        } while (i < text.Length);

        return result.ToString();
    }
}