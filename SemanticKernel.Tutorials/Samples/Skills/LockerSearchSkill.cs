using Microsoft.SemanticKernel.SkillDefinition;

namespace SemanticKernel.Tutorials.Samples.Skills;

internal class LockerSearchSkill
{
    private static readonly HttpClient _client = new();

    [SKFunction("Baggage locker search API")]
    [SKFunctionInput(DefaultValue = "Tokyo", Description = "Locker locations")]
    public async Task<string> SearchLocker(string lockerLocation)
    {
        const string url = @"http://localhost:7071/api/Function1";
        var response = await _client.PostAsync(url, new StringContent(lockerLocation));

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}