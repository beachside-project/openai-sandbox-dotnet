using Microsoft.DeepDev;

namespace SemanticKernelSamples.CognitiveSearchMemorySample;

internal class Helpers
{
    private const string ImStart = "<|im_start|>";
    private const string ImEnd = "<|im_end|>";
    private static readonly ITokenizer Tokenizer;
    private static readonly Dictionary<string, int> SpecialTokens = new()
    {
        { ImStart, 100264},
        { ImEnd, 100265},
    };

    static Helpers()
    {
        Tokenizer = TokenizerBuilder.CreateByModelNameAsync("text-embedding-ada-002", SpecialTokens).GetAwaiter().GetResult();
    }

    internal static int GetTokenCount(string text)
    {
        // NuGet: Microsoft.DeepDev.TokenizerLib
        var source = $"{ImStart}{text}{ImEnd}";
        var encoded = Tokenizer.Encode(source, new HashSet<string>(SpecialTokens.Keys));

        // decode 方法
        //var decoded = Tokenizer.Decode(encoded.ToArray());
        //Console.WriteLine(decoded);

        return encoded.Count;
    }
}