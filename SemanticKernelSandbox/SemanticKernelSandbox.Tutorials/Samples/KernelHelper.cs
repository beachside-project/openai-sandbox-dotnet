using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace SemanticKernel.Tutorials.Samples;

internal class KernelHelper
{
    private const string StartDelimiter = ">>>>>>>>>>";
    private const string EndDelimiter = "<<<<<<<<<<";
    internal static void LogPlan(Plan plan)
    {
        plan.ToJson();
        // {plan.ToJson(indented:true)} だと Encoder ががががが！
        Console.WriteLine($"""
            {StartDelimiter}
            PLAN:
            {JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) })}
            {EndDelimiter}

            """);
    }

    internal static async Task RunAsync(IKernel kernel, Plan plan, bool needStepRun = false)
    {
        if (needStepRun)
        {
            Console.WriteLine(StartDelimiter);
            while (plan.HasNextStep)
            {
                var index = plan.NextStepIndex; // current step はないので、実行前に NextStepIndex を取得
                await kernel.StepAsync(plan);
                Console.WriteLine($"""
                * STEP {index} - NAME: {plan.Steps[index].Name}; SKILL NAME: {plan.Steps[index].SkillName};
                {plan.State.Input}

                """);
            }

            Console.WriteLine(EndDelimiter);
        }
        else
        {
            var context = await kernel.RunAsync(plan);
            Console.WriteLine(@$"実行結果:
                                {context}
                              ");
        }
    }
}