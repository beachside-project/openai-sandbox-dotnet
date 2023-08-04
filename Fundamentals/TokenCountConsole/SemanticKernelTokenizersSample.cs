using Microsoft.SemanticKernel.Connectors.AI.OpenAI.Tokenizers;

namespace TokenCountConsole;

internal class SemanticKernelTokenizersSample
{
    internal void Run()
    {
        const string sampleText = "こんにちは、猫さん"; ;
        // 現状、Tokenizer はこれしかない。encoder は p50k_base の encode のみ (davinci や code-davinci に使われている) で、
        // GPT3.5/4 や ada-002 に使われている cl100k_base ではないので古い。。。
        // これは使わず素直に Tokenizer 使いましょうって流れです。
        // https://github.com/microsoft/Tokenizer/blob/main/Tokenizer_C%23/TokenizerLib/TokenizerBuilder.cs
        var count = GPT3Tokenizer.Encode(sampleText);
        Console.WriteLine(count);
    }
}