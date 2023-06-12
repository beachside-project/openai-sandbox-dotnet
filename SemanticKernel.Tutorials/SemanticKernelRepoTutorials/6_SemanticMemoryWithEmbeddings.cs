using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace SemanticKernel.Tutorials.SemanticKernelRepoTutorials;

internal class _6_SemanticMemoryWithEmbeddings
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        var kernel = new KernelBuilder()
            .WithAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey)
            .WithAzureTextEmbeddingGenerationService(options.DeploymentNameForEmbeddings, options.Endpoint, options.ApiKey)
            // Semantic Kernel で用意されている In-memory storage
            .WithMemoryStorage(new VolatileMemoryStore())
            .Build();

        //await UseSimpleInMemoryAsync(kernel);
        //await UseTextMemorySkillAsync(kernel);
        await UseUrlReferenceMemorySample(kernel);
    }

    /// <summary>
    /// In-memory の VolatileMemoryStore をシンプルに利用するサンプル
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    private static async Task UseSimpleInMemoryAsync(IKernel kernel)
    {
        const string memoryCollectionName = "aboutMe";

        // SaveInformationAsync: データを In-memory へ保存
        // public async Task<string> SaveInformationAsync(
        //     string collection,
        //     string text,
        //     string id,
        //     string? description = null,
        //     string? additionalMetadata = null,
        //     CancellationToken cancellationToken = default)
        // text が embeddings の対象っぽい
        await kernel.Memory.SaveInformationAsync(memoryCollectionName, id: "info1", text: "My name is BEACHSIDE");
        await kernel.Memory.SaveInformationAsync(memoryCollectionName, id: "info2", text: "I currently work as a 'Fortnite' warrior");
        await kernel.Memory.SaveInformationAsync(memoryCollectionName, id: "info3", text: "I currently live in Tokyo");
        await kernel.Memory.SaveInformationAsync(memoryCollectionName, id: "info4", text: "I visited France and Italy five times since 2015");
        await kernel.Memory.SaveInformationAsync(memoryCollectionName, id: "info5", text: "My family is from Hokkaido, Japan");

        var questions = new[]
        {
            "what is my name?",
            "where do I live?",
            "where have I traveled?",
            "what do I do for work?",
            "where is my family from?",
        };

        foreach (var question in questions)
        {
            var response = kernel.Memory.SearchAsync(memoryCollectionName, question);
            var answer = await response.FirstOrDefaultAsync();
            Console.WriteLine($"""
                Q: {question}
                A: {answer.Metadata.Text}
                """);
        }
    }

    /// <summary>
    /// kernel.Memory.SaveReferenceAsync のサンプル。
    /// 外部リンクを保持する用のフィールドがあるだけでリンクの中身みるわけではない。
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    private static async Task UseSaveReferenceAsync(IKernel kernel)
    {
        kernel.ImportSkill(new TextMemorySkill());

        const string memoryCollectionName = "aboutMe";
        // Prompt の中で 以下のように recall をコールするとそのキーの値が取得できる
        const string skPrompt = """
                        ChatBot can have a conversation with you about any topic.
                        It van git explicit instructions or say "I don(t know " if dose not have an answer.
                    
                        Information about me, from previous conversations:
                        - {{$fact1}} {{recall $fact1}}
                        - {{$fact2}} {{recall $fact2}}
                        - {{$fact3}} {{recall $fact3}}
                        - {{$fact4}} {{recall $fact4}}
                        - {{$fact5}} {{recall $fact5}}

                        Chat:
                        {{$history}}
                        User: {{$userInput}}
                        ChatBot: ";

                        """;


        var chatFunction = kernel.CreateSemanticFunction(skPrompt, maxTokens: 200, temperature: 0.8);

        var context = kernel.CreateNewContext();
        context["fact1"] = "what is my name?";
        context["fact2"] = "where do I live?";
        context["fact4"] = "where have I traveled?";
        context["fact5"] = "what do I do for work?";

        context[TextMemorySkill.CollectionParam] = memoryCollectionName;
        // これは0.0 - 1.0 で設定する。1.0 だと完全一致を対象とする。
        context[TextMemorySkill.RelevanceParam] = "0.8";


        var history = "";
        context["history"] = history;

        async Task Chat(string input)
        {
            // Context へ質問を保存
            context["userInput"] = input;
            // 質問の回答を取得
            var answer = await chatFunction.InvokeAsync(context);

            // chat の history に新しい回答を追加
            history += $"""
                User: {input}
                ChatBot: {answer}
                """;
            context["history"] = history;

            Console.WriteLine($"ChatBot: {answer}");
        }

        await Chat("Hello, I think we've met before, remember? my name is...");
        await Chat("I want to plan a trip and visit my family. Do you know where that is?");
        await Chat("Great! What are some fun things to do there?");
    }

    private static async Task UseUrlReferenceMemorySample(IKernel kernel)
    {
        const string memoryCollectionName = "SKGitHub";

        var githubData = new Dictionary<string, string>()
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
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/dotnet/KernelHttpServer/README.md"]
                = "README: How to set up a Semantic Kernel Service API using Azure Function Runtime v4",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/apps/chat-summary-webapp-react/README.md"]
                = "README: README associated with a sample starter react-based chat summary webapp",
        };

        kernel.ImportSkill(new TextMemorySkill());

        foreach (var data in githubData)
        {
            // SaveReferenceAsync: データを In-memory へ保存
            // public async Task<string> SaveReferenceAsync(
            //     string collection,
            //     string text,
            //     string externalId,
            //     string externalSourceName,
            //     string? description = null,
            //     string? additionalMetadata = null,
            //     CancellationToken cancellationToken = default)
            // SaveInformationAsync との違いは、externalId, externalSourceName があるくらいで大差は無さそう。
            // text が embeddings の対象っぽい
            await kernel.Memory.SaveReferenceAsync(collection: memoryCollectionName,
                description: data.Value,  // 必須ではない
                text: data.Value,
                externalId: data.Key,
                externalSourceName: "GitHub");
        }

        string ask = "I love Jupyter notebooks, how should I get started?";
        Console.WriteLine($"""
                         ===========================
                         "Query: ": {ask} 
                         """);

        var memoryQueryResults = kernel.Memory.SearchAsync(memoryCollectionName, ask, limit: 5, minRelevanceScore: 0.7);

        await foreach (var result in memoryQueryResults)
        {
            Console.WriteLine($"""
                URL: {result.Metadata.Id}
                Title: {result.Metadata.Description}
                Relevance: {result.Relevance}
                """);
        }


    }
}