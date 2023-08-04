using Microsoft.DeepDev;

namespace TokenCountConsole;

public class TokenizerSample
{
    private const string ImStart = "<|im_start|>";
    private const string ImEnd = "<|im_end|>";

    private static readonly Dictionary<string, int> SpecialTokens = new()
    {
        { ImStart,100264},
        { ImEnd, 100265},
    };

    private static readonly IReadOnlyCollection<string> AllowedSpecial = new HashSet<string>(SpecialTokens.Keys);

    public async Task RunAsync()
    {
        var text = "こんにちは、猫さん";

        // gpt-3.5-turbo
        var count = await CountTokenAsync(text, "gpt-3.5-turbo");
        Console.WriteLine(count);

        // davini 系など
        var count2 = await CountTokenByEncoder(text, "p50k_base");
        Console.WriteLine(count2);
    }

    public async Task<int> CountTokenAsync(string text, string modelName)
    {
        var tokenizer = await TokenizerBuilder.CreateByModelNameAsync(modelName, SpecialTokens);
        var input = $"{ImStart}{text}{ImEnd}";
        var encoded = tokenizer.Encode(input, AllowedSpecial);
        return encoded.Count - 2;
    }

    public async Task<int> CountTokenByEncoder(string text, string encoderName)
    {
        var tokenizer = await TokenizerBuilder.CreateByEncoderNameAsync(encoderName, SpecialTokens);
        var encoded = tokenizer.Encode($"{ImStart}{text}{ImEnd}", AllowedSpecial);
        return encoded.Count - 2;
    }
}