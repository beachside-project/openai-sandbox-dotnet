using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace SemanticKernel.Tutorials.SemanticKernelRepoTutorials;

internal class _4_ContextVariablesChat
{
    internal static async Task RunAsync(SemanticKernelOptions options)
    {
        var kernel = new KernelBuilder()
            .WithAzureTextCompletionService(options.DeploymentNameForTextCompletion, options.Endpoint, options.ApiKey)
            .Build();

        const string skPrompt = @"""
                Assistant can have a conversation with you about any topic.
                It can give explicit instructions or say 'I don't know' if it does not have an answer.

                {{$history}}

                User: {{$userInput}}
                Assistant:
                """;
        var promptConfig = new PromptTemplateConfig
        {
            Completion =
            {
                MaxTokens = 2000,
                Temperature = 0.7
            }
        };

        var promptTemplate = new PromptTemplate(skPrompt, promptConfig, kernel);
        var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);
        var chatFunction = kernel.RegisterSemanticFunction("ChatBot", "Chat", functionConfig);

        var context = kernel.CreateNewContext();
        context["history"] = string.Empty;

        var userInput = "Hi, I'm looking for suggestions";
        context["userInput"] = userInput;

        var botAnswer = await chatFunction.InvokeAsync(context);

        var history = $"""

            User: {userInput};
            Assistant: {botAnswer}
            """;
        context.Variables.Update(history);

        async Task Chat(string input)
        {
            context["userInput"] = input;
            var answer = await chatFunction.InvokeAsync(context);

            history += $"""

            User: {input};
            Assistant: {answer}
            """;
            context["history"] = history;
            Console.WriteLine(context);
        }

        await Chat("I would like a non-fiction book suggestion about Greece history. Please only list one book.");
        await Chat("that sounds interesting, what are some of the topics I will learn about?");
        await Chat("Which topic from the ones you listed do you think most people find interesting?");
        await Chat("could you list some more books I could read about the topic(s) you mentioned?");
    }
}