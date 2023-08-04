using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using SemanticKernel.Tutorials.GetStarted.SemanticPlugins;

namespace SemanticKernel.Tutorials.GetStarted;

// docs: https://learn.microsoft.com/en-us/semantic-kernel/create-chains/native-functions
internal class _2_ChainingNativeFunctions
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        var kernel = Kernel.Builder
            .Configure(config =>
            {
                config.AddAzureTextCompletionService(options.DeploymentNameForTextCompletion,
                    options.Endpoint,
                    options.ApiKey);
            }).Build();

        //SemanticKernel.Tutorials.GetStarted.SemanticPlugins.SamplePlugin1.cs
        var samplePlugin1 = kernel.ImportSkill(new SamplePlugin1(), nameof(SamplePlugin1));

        var variables = new ContextVariables();
        //variables.Set("min", "50");
        //variables.Set("max", "200");
        //variables.Set("text", "elle");
        variables.Set("INPUT", "elle");

        var output1 = await kernel.RunAsync(variables, samplePlugin1["GetRandomNumber"]);
        Console.WriteLine($"""
                回答:
                ErrorOccurred: {output1.ErrorOccurred} ({output1.LastErrorDescription})
                Result: {output1.Result}
                """);

        var output2 = await kernel.RunAsync(variables, samplePlugin1["DupDup"]);
        Console.WriteLine($"""
                回答:
                ErrorOccurred: {output2.ErrorOccurred} ({output2.LastErrorDescription})
                Result: {output2.Result}
                """);
    }
}