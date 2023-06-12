using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;

namespace SemanticKernel.Tutorials.GetStarted;

internal class _1_GetStarted
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        // v0.14 系
        var kernel = Kernel.Builder
            .Configure(config =>
            {
                config.AddAzureTextCompletionService(options.DeploymentNameForTextCompletion,
                    options.Endpoint,
                    options.ApiKey);
            }).Build();

        await RunSimpleSemanticFunctionAsync(kernel);


    }

    private static async Task RunSimpleSemanticFunctionAsync(IKernel kernel)
    {
        var prompt = """
                ```text
                {{$input}}
                ```

                text を20文字程度で要約してください。

                要約:

            """;

        ISKFunction summarize = kernel.CreateSemanticFunction(prompt);

        var context = await summarize.InvokeAsync("""
                春はあけぼの。やうやう白くなりゆく山ぎは、すこしあかりて、紫だちたる 雲のほそくたなびきたる。
                夏は夜。月のころはさらなり。やみもなほ、蛍の多く飛びちがひたる。また、 ただ一つ二つなど、ほのかにうち光りて行くもをかし。雨など降るもをかし。
                秋は夕暮れ。夕日のさして山の端いと近うなりたるに、烏の寝どころへ行く とて、三つ四つ、二つ三つなど、飛びいそぐさへあはれなり。まいて雁などの つらねたるが、いと小さく見ゆるはいとをかし。日入りはてて、風の音、虫の 音など、はたいふべきにあらず。
                冬はつとめて。雪の降りたるはいふべきにもあらず、霜のいと白きも、また さらでもいと寒きに、火など急ぎおこして、炭もて渡るもいとつきづきし。 昼になりて、ぬるくゆるびもていけば、火桶の火も白き灰がちになりてわろし。
                """);

        Console.WriteLine($"""
                回答:
                ErrorOccurred: {context.ErrorOccurred} ({context.LastErrorDescription})
                Result: {context.Result}
                """);

    }

}