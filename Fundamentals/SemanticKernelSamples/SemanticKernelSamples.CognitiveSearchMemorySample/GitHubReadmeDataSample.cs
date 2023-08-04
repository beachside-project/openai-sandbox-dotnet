using Microsoft.SemanticKernel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace SemanticKernelSamples.CognitiveSearchMemorySample;

/// <summary>
/// SaveReferenceAsync で cognitive search に保存したデータに対する検索。
/// この公式ブログのサンプル:
/// https://devblogs.microsoft.com/semantic-kernel/announcing-semantic-kernel-integration-with-azure-cognitive-search/
/// </summary>
internal class GitHubReadmeDataSample
{
    private const string CognitiveSearchIndexName = "GitHubFiles";
    internal static async Task RunAsync(IKernel kernel)
    {
        await UpsertIndexAsync(kernel);
        await SearchAsync(kernel);
    }

    private static async Task UpsertIndexAsync(IKernel kernel)
    {
        var gitHubFiles = new Dictionary<string, string>
        {
            ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
      = "README: Installation, getting started, and how to contribute",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/02-running-prompts-from-file.ipynb"]
      = "Jupyter notebook describing how to pass prompts from a file to a semantic skill or function",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/00-getting-started.ipynb"]
      = "Jupyter notebook describing how to get started with the Semantic Kernel",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/ChatSkill/ChatGPT"]
      = "Sample demonstrating how to create a chat skill interfacing with ChatGPT",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel/Memory/VolatileMemoryStore.cs"]
      = "C# class that defines a volatile embedding store",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet/KernelHttpServer/README.md"]
      = "README: How to set up a Semantic Kernel Service API using Azure Function Runtime v4",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md"]
      = "README: README associated with a sample chat summary react-based webapp",
        };

        foreach (var entry in gitHubFiles)
        {

            await kernel.Memory.SaveReferenceAsync(
                collection: CognitiveSearchIndexName,
                externalSourceName: "GitHub",
                externalId: entry.Key,
                description: entry.Value,
                text: entry.Value);
        }
    }

    private static async Task SearchAsync(IKernel kernel)
    {
        var memories = kernel.Memory.SearchAsync(CognitiveSearchIndexName, "Can I build a chat with SK?", limit: 3);
        await foreach (var memory in memories)
        {
            //Console.WriteLine("URL:     : " + memory.Metadata.Id);
            //Console.WriteLine("Title    : " + memory.Metadata.Description);
            //Console.WriteLine("Relevance: " + memory.Relevance);

            Console.WriteLine(JsonSerializer.Serialize(memory, options: new()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            }));
        }
    }
}