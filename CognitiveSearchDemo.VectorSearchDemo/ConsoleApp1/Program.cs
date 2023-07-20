namespace ConsoleApp1;

internal class Program
{
    static async Task Main()
    {
        var options = AppOptions.ReadAppConfigFromUserSecrets();
        var indexApp = new IndexManagement();
        await indexApp.RunAsync(options);

        var docApp = new DocumentManagement();
        await docApp.RunAsync(options);


        var search = new SearchOperations();
        await search.RunAsync(options);

    }
}