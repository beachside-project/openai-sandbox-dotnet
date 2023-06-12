using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using System.Text;
using System.Text.Json;

namespace SemanticKernel.Tutorials.SemanticKernelRepoTutorials;

internal class _5_UsingPlanner
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        var kernel = new KernelBuilder()
            .WithAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey)
            .Build();

        var planner = new SequentialPlanner(kernel);

        // Planner が利用できる Skill を登録
        //var skillDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SemanticKernelRepoTutorials", "Skills");
        var skillDirectory = Path.Combine(@"C:\repos\azure-openai\openai-sandbox-dotnet\SemanticKernel.Tutorials", "SemanticKernelRepoTutorials", "Skills");
        kernel.ImportSemanticSkillFromDirectory(skillDirectory, skillDirectoryNames: "SummarizeSkill");
        kernel.ImportSemanticSkillFromDirectory(skillDirectory, "WriterSkill");

        var ask1 = "Tomorrow is Valentine's day. I need to come up with a few date ideas and e-mail them to my significant other.";

        // plan の自動生成
        var originalPlan = await planner.CreatePlanAsync(ask1);

        Console.WriteLine($"""
            originalPlan:
            {JsonSerializer.Serialize(originalPlan, new JsonSerializerOptions { WriteIndented = true })}
            """);

        // 指定したスキルのみで plan を生成
        var skPrompt = """
            data:
            ---
            {{$input}}
            ---

            data をシェークスピア風に書き直して、日本語で回答してください。
            """;

        //
        kernel.CreateSemanticFunction(skPrompt, functionName: "shakespeare", skillName: "ShakespeareSkill", maxTokens: 2000, temperature: 0.2, topP: 0.5);

        var ask2 = "Tomorrow is Valentine's day. I need to come up with a few date ideas and e-mail them to my significant other. Write email in Shakespeare style";

        var newPlan = await planner.CreatePlanAsync(ask2);
        Console.WriteLine($"""
            newPlan:
            {JsonSerializer.Serialize(newPlan, new JsonSerializerOptions { WriteIndented = true })}
            """);

        // plan の実行

        var originalPlanResult = await originalPlan.InvokeAsync();
        Console.WriteLine($"""
            Original plan result:
            {WordWrap(originalPlanResult.Result, 100)}
        """);

        var newPlanResult = await newPlan.InvokeAsync();
        Console.WriteLine($"""
            new plan result:
            {WordWrap(newPlanResult.Result, 100)}
        """);

    }

    // Function used to wrap long lines of text
    public static string WordWrap(string text, int maxLineLength)
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