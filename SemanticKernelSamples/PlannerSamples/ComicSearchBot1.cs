using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Skills.Web;
using Microsoft.SemanticKernel.Skills.Web.Bing;
using System.Text;
using System.Text.Json;

namespace PlannerSamples;

internal class ComicSearchBot1
{
    internal async Task RunAsync()
    {
        var options = SemanticKernelOptions.CreateClientFromUserSecrets();

        var kernel = Kernel.Builder
            .WithAzureChatCompletionService(options.DeploymentNameForChatCompletion, options.Endpoint, options.ApiKey)
            .WithAzureTextEmbeddingGenerationService(options.DeploymentNameForEmbeddings, options.Endpoint, options.ApiKey)
            .Build();
        await CreatePlanner(kernel, options);
    }

    private async Task CreatePlanner(IKernel kernel, SemanticKernelOptions options)
    {
        // Bing Connector
        // - NuGet: Microsoft.SemanticKernel.Skills.Web
        kernel.ImportSkill(new WebSearchEngineSkill(new BingConnector(options.BingWebSearchKey)), "bing");

        // Semantic Function - main prompt
        //var skPrompt = kernel.CreateSemanticFunction(
        //    """
        //    あなたのタスクは、QUESTION タグ内の文章から、REFERENCE タグの情報を元に集英社のマンガのタイトルを回答することです。
        //    マンガのタイトルだけを回答して答えてください。
        //    複数の候補がある場合は複数のマンガのタイトルを回答してください。
        //    REFERENCE タグ内の情報以外は回答に含めてはいけません。REFERENCE タグ内の情報から回答が見つからない場合は「わかりません。もう少し情報を教えてください」返答してください。

        //    <QUESTION>
        //    {{$question}}
        //    </QUESTION>

        //    <REFERENCE>
        //    {{bing.search $question }}
        //    </REFERENCE>

        //    回答:
        //    """
        //    //, maxTokens: 4000
        //    );

        var skPrompt = kernel.CreateSemanticFunction(
            """
            あなたのタスクは、QUESTION タグ内の文章から、REFERENCE タグの情報を元に講談社のマンガのタイトルを回答することです。
            マンガのタイトルだけを回答して答えてください。
            複数の候補がある場合は複数のマンガのタイトルを回答してください。
            REFERENCE タグ内の情報以外は回答に含めてはいけません。REFERENCE タグ内の情報から回答が見つからない場合は「わかりません。もう少し情報を教えてください」返答してください。

            <QUESTION>
            {{$question}}
            </QUESTION>

            <REFERENCE>
            {{bing.search $question }}
            </REFERENCE>

            回答:
            """
        //, maxTokens: 4000
        );

        // TODO:
        //var question = "ラブコメ";
        var question = "家が全焼する炎上事件から村田杏子が御手洗家に潜入して復讐をする話";

        //const string defaultWord = "講談社 マンガ 人気 wikipedia ";
        const string defaultWord = "講談社 マンガ ";

        var variables = new ContextVariables();
        variables.Set("question", defaultWord + question);

        // 検索結果の数を指定 (default: 1)
        variables.Set(WebSearchEngineSkill.CountParam, "10");

        // prompt を直接実行
        var answer = await kernel.RunAsync(variables, skPrompt);
        Console.WriteLine(answer.Result);



    }
}