using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace ConsoleApp1;

internal class IndexManagement
{
    internal async Task RunAsync(AppOptions options)
    {
        //await CreateIndexAsync(options);
    }

    private async Task CreateIndexAsync(AppOptions options)
    {
        var credentials = new AzureKeyCredential(options.CognitiveSearchAdminKey);
        var indexClient = new SearchIndexClient(new Uri(options.CognitiveSearchEndpoint), credentials);

        // Update は、既存のフィールドを変更できるわけではない。追加だけ。
        var response = await indexClient.CreateOrUpdateIndexAsync(GetSampleIndex(options.CognitiveSearchIndexName));
        Console.WriteLine(response.GetRawResponse().Status);
    }

    private SearchIndex GetSampleIndex(string name)
    {
        const string vectorSearchConfigName = "vectorConfig1";
        const string semanticSearchConfigName = "vectorConfig1";

        SearchIndex searchIndex = new(name)
        {
            VectorSearch = new()
            {
                AlgorithmConfigurations =
                {
                    new HnswVectorSearchAlgorithmConfiguration(vectorSearchConfigName)
                }
            },
            SemanticSettings = new()
            {
                Configurations =
                {
                   new SemanticConfiguration(semanticSearchConfigName, new()
                   {
                       TitleField = new(){ FieldName = "title" },
                       ContentFields =
                       {
                           new() { FieldName = "content" }
                       },
                       //KeywordFields =
                       //{
                       //    new() { FieldName = "category" }
                       //}
                   })
                }
            },
            CorsOptions = new(new []{"*"})
            {
                MaxAgeInSeconds = 60
            },
            Fields =
            {
                new SimpleField("id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
                new SearchableField("category") { IsKey = false, IsFilterable = true,IsSortable = false, IsFacetable = true,},
                new SearchableField("title") { IsFilterable = true, IsSortable = true },
                new SearchField("titleVector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    IsSearchable = true,
                    VectorSearchDimensions = 1536,
                    VectorSearchConfiguration = vectorSearchConfigName
                },
                new SearchableField("content") { IsFilterable = true },
                new SearchField("contentVector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    IsSearchable = true,
                    VectorSearchDimensions = 1536,
                    VectorSearchConfiguration = vectorSearchConfigName
                }
            }
        };

        return searchIndex;
    }
}