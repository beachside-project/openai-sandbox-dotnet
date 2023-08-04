using Microsoft.DeepDev;

namespace SemanticKernelSandbox.TextSplitSamples;

public static class TokenCounter
{
    private const string ImStart = "<|im_start|>";
    private const string ImEnd = "<|im_end|>";

    private static readonly Dictionary<string, int> SpecialTokens = new()
    {
        { ImStart,100264},
        { ImEnd, 100265},
    };

    static TokenCounter()
    {
        DefaultTokenizer = TokenizerBuilder.CreateByModelNameAsync("gpt-3.5-turbo", SpecialTokens).GetAwaiter().GetResult();
    }

    private static ITokenizer DefaultTokenizer { get; }

    private static readonly IReadOnlyCollection<string> AllowedSpecial = new HashSet<string>(SpecialTokens.Keys);

    public static async Task<int> CountTokenAsync(string text, string modelName = "gpt-3.5-turbo")
    {
        if (modelName == "gpt-3.5-turbo")
        {
            var input = $"{ImStart}{text}{ImEnd}";
            var encoded = DefaultTokenizer.Encode(input, AllowedSpecial);
            return encoded.Count - 2;
        }
        else
        {
            var tokenizer = await TokenizerBuilder.CreateByModelNameAsync(modelName, SpecialTokens);
            var input = $"{ImStart}{text}{ImEnd}";
            var encoded = tokenizer.Encode(input, AllowedSpecial);
            return encoded.Count - 2;
        }
    }
}