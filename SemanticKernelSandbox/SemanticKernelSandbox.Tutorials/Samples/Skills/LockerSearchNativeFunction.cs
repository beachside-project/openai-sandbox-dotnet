using Microsoft.SemanticKernel.SkillDefinition;
using System.ComponentModel;

namespace SemanticKernel.Tutorials.Samples.Skills;

internal class LockerSearchNativeFunction
{
    private static readonly HttpClient _client = new();

    //[SKFunction, Description("Baggage locker search API")]
    //[SKParameter("lockerLocation", "Locker locations", DefaultValue = "Tokyo")]
    //public async Task<string> SearchLocker(string lockerLocation)
    //{
    //    const string url = @"http://localhost:7071/api/Function1";
    //    var response = await _client.PostAsync(url, new StringContent(lockerLocation));

    //    var result = await response.Content.ReadAsStringAsync();
    //    return result;
    //}

    // ↑とこっちは同じ意味になるはず (未確認)
    [SKFunction, Description("Baggage locker search API")]
    public async Task<string> SearchLocker([Description("Locker locations")] string lockerLocation = "Tokyo")
    {
        const string url = @"http://localhost:7071/api/Function1";
        var response = await _client.PostAsync(url, new StringContent(lockerLocation));

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}