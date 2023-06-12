using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;

namespace SemanticKernel.Tutorials.SemanticKernelRepoTutorials;

internal class _2_HowToRunSemanticSkillsFromFile
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        await Sample1(options);
        await Sample2(options);
    }

    private static async Task Sample1(SemanticKernelOptions options)
    {
        // v0.14
        //var kernel = new KernelBuilder()
        //    .Configure(config =>
        //    {
        //        config.AddAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey);
        //    })
        //    .Build();

        // v0.15
        var kernel = new KernelBuilder()
            .WithAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey)
            .Build();

        var skillDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SemanticKernelRepoTutorials", "Skills");
        var funSkillFunctions = kernel.ImportSemanticSkillFromDirectory(skillDirectory, "FunSkill");

        var input = "time travel to dinosaur age";

        // settings は値を替えたときだけつければ OK
        var settings = new CompleteRequestSettings
        {
            Temperature = 1.2
        };
        var output = await funSkillFunctions["Joke"].InvokeAsync(input, settings);
        Console.WriteLine(output);
    }

    // 複数の引数を渡すには kernel.RunAsync を使うしかないのかわからん。
    private static async Task Sample2(SemanticKernelOptions options)
    {
        // v0.14
        //var kernel = new KernelBuilder()
        //    .Configure(config =>
        //    {
        //        config.AddAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey);
        //    })
        //    .Build();

        // v0.15
        var kernel = new KernelBuilder()
            .WithAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey)
            .Build();

        var skillDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SemanticKernelRepoTutorials", "Skills");
        var funSkillFunctions = kernel.ImportSemanticSkillFromDirectory(skillDirectory, "FunSkill");

        var variables = new ContextVariables()
        {
            ["input"] = "time travel to dinosaur age",
            ["maxCharacters"] = "50"
        };

        var output = await kernel.RunAsync(variables, funSkillFunctions["Joke"]);
        Console.WriteLine(output);
    }
}