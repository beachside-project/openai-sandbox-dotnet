// See https://aka.ms/new-console-template for more information

using OpenAISdkSamples;

var config = OpenAIOptions.ReadFromUserSecrets();
await ChatCompletionSample1.RunAsync(config);
