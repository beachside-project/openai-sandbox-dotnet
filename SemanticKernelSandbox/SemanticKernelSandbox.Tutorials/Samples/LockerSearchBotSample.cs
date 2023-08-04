using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using SemanticKernel.Tutorials.Samples.Skills;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace SemanticKernel.Tutorials.Samples;

internal class LockerSearchBotSample
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        var kernel = new KernelBuilder()
            .WithAzureChatCompletionService(options.DeploymentNameForChatCompletion, options.Endpoint, options.ApiKey)
            .Build();

        kernel.AddLockerSearchSkillSet();

        var question = "札幌駅で50番のロッカーは空いてるか";

        var planner = new SequentialPlanner(kernel);
        var plan = await planner.CreatePlanAsync(question);

        Console.WriteLine(@$"
            ++++++++++++++++++++++
            PLAN:
            {JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) })}
            ++++++++++++++++++++++

            ");

        await KernelHelper.RunAsync(kernel, plan, needStepRun: false);
    }
}