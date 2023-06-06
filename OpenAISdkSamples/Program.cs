// See https://aka.ms/new-console-template for more information

using OpenAISdkSamples;

Console.WriteLine("ChatCompletion sample");

var config = OpenAiConfig.ReadFromUserSecrets();

await ChatCompletionSample1.RunAsync(config);