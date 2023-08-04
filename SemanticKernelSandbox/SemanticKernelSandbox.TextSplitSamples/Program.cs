using Microsoft.SemanticKernel.Text;

namespace SemanticKernelSandbox.TextSplitSamples;

internal class Program
{
    // 結論:
    // 英語だと maxTokensPerLine で示した token 数以内で分割してくれている。
    // 日本語は全然だめかもしれん。maxTokensPerLine =200 に設定したら適当に400-700位で分割、500 にしたら2000オーバーする感じ。
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        //var filePath = @"C:\repos\azure-openai\openai-sandbox-dotnet\SemanticKernelSandbox\SemanticKernelSandbox.TextSplitSamples\english.txt";
        var filePath = @"C:\repos\azure-openai\openai-sandbox-dotnet\SemanticKernelSandbox\SemanticKernelSandbox.TextSplitSamples\magica.md";

        using var reader1 = new StreamReader(filePath);
        var sampleText = reader1.ReadToEnd();

        var sampleTexts = new List<string>();
        using (var reader2 = new StreamReader(filePath))
        {
            while (true)
            {
                var line = reader2.ReadLine();
                if (line == null) break;
                sampleTexts.Add(line);
            }
        };

        var maxTokensPerLine = 200;
        var overlapTokens = 0;

        // PlainText:

        var t1 = TextChunker.SplitPlainTextLines(sampleText, maxTokensPerLine);
        var t1TokensResult = await t1
                            .Select(async chunk => new { length = chunk.Length, Tokens = await TokenCounter.CountTokenAsync(chunk) })
                            .WhenAll();

        var t2 = TextChunker.SplitPlainTextParagraphs(sampleTexts, maxTokensPerLine, overlapTokens);
        var t2TokensResult = await t2
                            .Select(async chunk => new { length = chunk.Length, Tokens = await TokenCounter.CountTokenAsync(chunk) })
                            .WhenAll();

        // Markdown:

        var m1 = TextChunker.SplitMarkDownLines(sampleText, maxTokensPerLine);
        var m1TokensResult = await m1
                            .Select(async chunk => new { length = chunk.Length, Tokens = await TokenCounter.CountTokenAsync(chunk) })
                            .WhenAll();

        var m2 = TextChunker.SplitMarkdownParagraphs(sampleTexts, maxTokensPerLine, overlapTokens);
        var m2TokensResult = await m2
                            .Select(async chunk => new { length = chunk.Length, Tokens = await TokenCounter.CountTokenAsync(chunk) })
                            .WhenAll();
    }
}

public static class TaskEnumerableExtensions
{
    public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

    public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks) => Task.WhenAll(tasks);
}