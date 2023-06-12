using SemanticKernel.Tutorials;
using SemanticKernel.Tutorials.GetStarted;
using SemanticKernel.Tutorials.Samples;
using SemanticKernel.Tutorials.SemanticKernelRepoTutorials;

var options = SemanticKernelOptions.CreateClientFromUserSecrets();

//await _1_GetStarted.RunAsync(options);
//await _2_ChainingNativeFunctions.RunAsync(options);

//await _2_HowToRunSemanticSkillsFromFile.RunAsync(options);
//await _3_RunningSemanticFunctionsInline.RunAsync(options);
//await _4_ContextVariablesChat.RunAsync(options);
//await _5_UsingPlanner.RunAsync(options);
//await _6_SemanticMemoryWithEmbeddings.RunAsync(options);


//await SequentialPlannerSample1.RunAsync(options);
await SequentialPlannerSample2.RunAsync(options);