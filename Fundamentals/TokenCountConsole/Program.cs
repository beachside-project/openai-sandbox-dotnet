namespace TokenCountConsole;

internal class Program
{
    static async Task Main()
    {
        var options = OpenAIOptions.ReadFromUserSecrets();

        await new TokenizerSample().RunAsync();
        new SemanticKernelTokenizersSample().Run();
    }
}