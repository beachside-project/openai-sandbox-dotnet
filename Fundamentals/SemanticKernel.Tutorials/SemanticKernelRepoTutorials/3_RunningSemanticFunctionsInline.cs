using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace SemanticKernel.Tutorials.SemanticKernelRepoTutorials;

internal class _3_RunningSemanticFunctionsInline
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        await RunWithMultipleInputs(options);
    }


    private static async Task RunWithSingleVariable(SemanticKernelOptions options)
    {
        var kernel = new KernelBuilder()
            .Configure(config =>
            {
                config.AddAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey);
            })
            .Build();

        var prompt = """

                {{$input}}

                Summarize the context above in Japanese.
                """;

        var promptConfig = new PromptTemplateConfig()
        {
            Completion =
            {
                MaxTokens = 2000,
                Temperature = 0,
                TopP = 0.5
            }
        };

        var promptTemplate = new PromptTemplate(prompt, promptConfig, kernel);
        var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);
        var summaryFunction = kernel.RegisterSemanticFunction("MySkill", "SummarizeText", functionConfig);

        var input = """
                        Demo (ancient Greek poet)
                        From Wikipedia, the free encyclopedia
                        Demo or Damo (Greek: Δεμώ, Δαμώ; fl. c. AD 200) was a Greek woman of the Roman period, known for a single epigram, engraved upon the Colossus of Memnon, which bears her name. She speaks of herself therein as a lyric poetess dedicated to the Muses, but nothing is known of her life.[1]
                        Identity
                        Demo was evidently Greek, as her name, a traditional epithet of Demeter, signifies. The name was relatively common in the Hellenistic world, in Egypt and elsewhere, and she cannot be further identified. The date of her visit to the Colossus of Memnon cannot be established with certainty, but internal evidence on the left leg suggests her poem was inscribed there at some point in or after AD 196.[2]
                        Epigram
                        There are a number of graffiti inscriptions on the Colossus of Memnon. Following three epigrams by Julia Balbilla, a fourth epigram, in elegiac couplets, entitled and presumably authored by "Demo" or "Damo" (the Greek inscription is difficult to read), is a dedication to the Muses.[2] The poem is traditionally published with the works of Balbilla, though the internal evidence suggests a different author.[1]
                        In the poem, Demo explains that Memnon has shown her special respect. In return, Demo offers the gift for poetry, as a gift to the hero. At the end of this epigram, she addresses Memnon, highlighting his divine status by recalling his strength and holiness.[2]
                        Demo, like Julia Balbilla, writes in the artificial and poetic Aeolic dialect. The language indicates she was knowledgeable in Homeric poetry—'bearing a pleasant gift', for example, alludes to the use of that phrase throughout the Iliad and Odyssey.[a][2]
                        """;

        var output = await summaryFunction.InvokeAsync(input);
        Console.WriteLine(output);

    }

    private static async Task RunWithMultipleInputs(SemanticKernelOptions options)
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

        var prompt = """

                {{$input}}

                Summarize the context above about {{$maxCharacters}} characters in Japanese.
                """;

        var promptConfig = new PromptTemplateConfig()
        {
            Completion =
            {
                MaxTokens = 2000,
                // Temperature:
                // GPT-4 だとデフォルトは1。サンプリング温度は0～2の間で指定します。0.8のような高い値は出力をよりランダムにし、0.2のような低い値は出力をより集中させて、決定論的にします。
                // GPT3だと0-1はあまりかわらんっぽい。GPT4だと結構変わる。
                // 1より大きいと文章が破綻する傾向有り。
                Temperature = 0.2,
                // TopP (top_p):
                // GPT-4などのニューラルネットワーク言語モデルの出力を制御するために使用されるパラメーターで、モデルが生成する文章の多様性と品質を調整するために使われます。
                // 核サンプリングとも呼ばれています。
                // 出力されるトークンの確率分布から、トークンを選択する際の累積確率の閾値です。
                // 例えば、top_pを0.9に設定すると、モデルが選択するトークンの累積確率が90％になるように調整されます。
                // TopPが低いと、モデルは確率の高いトークンを選択しやすくなり、一貫性と品質が高くなりますが、文章の多様性が低くなって、同じような文章が繰り返されることが多くなります。
                // 逆にTopPが高いと、文章の多様性が増しますが、一貫性と品質が低下して、文法的におかしくなったり、文章が理解しにくくなったりするおそれがあります。
                // TopP の調整は、生成されるテキストの品質と多様性に大きな影響を与えるので、特定のタスクや目的に適した値を選択することが重要です。
                // Temperature と TopP:
                // Temperatureも、TopPと同じように言語モデルの出力を制御するためのパラメーターであり、多様性と品質の調整に使用されます。
                // ただし、temperatureは確率分布全体をスケーリングするのに対して、TopPはトークンの確率分布からトークンを選択する際の閾値を設定します。
                // temperatureが低いと、確率の高いトークンを選択しやすくなり、高いと、モデルはよりランダムなトークンを選択しやすくなります。
                // 一方、TopPは、選択されるトークンの累積確率に基づいて多様性を制御します。
                TopP = 0.5
                // PresencePenalty
                // デフォルトは0。-2.0から2.0の間の数値。
                // 正の値は、新しいトークンがこれまでのテキストに出現したトークンを繰り返すとペナルティを与え、新しいトピックについて話す可能性を高めます。
                //, PresencePenalty = 0
                // FrequencyPenalty:
                // デフォルトは0。-2.0から2.0の間の数値。
                // 正の値は、これまでのテキストにおける頻度に基づいて新しいトークンにペナルティを与え、モデルが同じ行をそのまま繰り返す可能性を減少させます。
                //,FrequencyPenalty = 0,

            }
        };

        var promptTemplate = new PromptTemplate(prompt, promptConfig, kernel);
        var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);
        var summaryFunction = kernel.RegisterSemanticFunction("MySkill", "SummarizeText", functionConfig);

        var input = """
                        Demo (ancient Greek poet)
                        From Wikipedia, the free encyclopedia
                        Demo or Damo (Greek: Δεμώ, Δαμώ; fl. c. AD 200) was a Greek woman of the Roman period, known for a single epigram, engraved upon the Colossus of Memnon, which bears her name. She speaks of herself therein as a lyric poetess dedicated to the Muses, but nothing is known of her life.[1]
                        Identity
                        Demo was evidently Greek, as her name, a traditional epithet of Demeter, signifies. The name was relatively common in the Hellenistic world, in Egypt and elsewhere, and she cannot be further identified. The date of her visit to the Colossus of Memnon cannot be established with certainty, but internal evidence on the left leg suggests her poem was inscribed there at some point in or after AD 196.[2]
                        Epigram
                        There are a number of graffiti inscriptions on the Colossus of Memnon. Following three epigrams by Julia Balbilla, a fourth epigram, in elegiac couplets, entitled and presumably authored by "Demo" or "Damo" (the Greek inscription is difficult to read), is a dedication to the Muses.[2] The poem is traditionally published with the works of Balbilla, though the internal evidence suggests a different author.[1]
                        In the poem, Demo explains that Memnon has shown her special respect. In return, Demo offers the gift for poetry, as a gift to the hero. At the end of this epigram, she addresses Memnon, highlighting his divine status by recalling his strength and holiness.[2]
                        Demo, like Julia Balbilla, writes in the artificial and poetic Aeolic dialect. The language indicates she was knowledgeable in Homeric poetry—'bearing a pleasant gift', for example, alludes to the use of that phrase throughout the Iliad and Odyssey.[a][2]
                        """;

        var variables = new ContextVariables();
        variables.Set("input", input);
        variables.Set("maxCharacters", "100");

        var output = await kernel.RunAsync(variables, summaryFunction);
        Console.WriteLine(output);
    }



}