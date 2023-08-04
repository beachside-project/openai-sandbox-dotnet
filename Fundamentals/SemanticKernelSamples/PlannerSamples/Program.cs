namespace PlannerSamples;

internal class Program
{
    static async Task Main()
    {
        //var app = new ComicSearchBot1();
        var app = new ComicSearchBot2();
        await app.RunAsync();
    }
}