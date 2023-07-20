using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace ConsoleApp1;

internal class IndexManagement
{
    internal async Task RunAsync(AppOptions options)
    {
        await CreateIndexAsync(options);
    }

    private async Task CreateIndexAsync(AppOptions options)
    {
        var credentials = new AzureKeyCredential(options.CognitiveSearchAdminKey);
        var indexClient = new SearchIndexClient(new Uri(options.CognitiveSearchEndpoint), credentials);

        var indexToCreate = GetSampleIndex(options.CognitiveSearchIndexName);
        // Update は、既存のフィールドを変更できるわけではない。追加だけ。
        var response = await indexClient.CreateOrUpdateIndexAsync(indexToCreate);
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
            // ※ Field 属性について (true の場合の説明)
            // searchable: 言語アナライザーを使ったフルテキスト検索を有効にする。
            // filterable: 完全一致の比較が可能か。
            // sortable: このフィールドでソートを可能にする。Collection(Edm.String) の field は利用不可。
            // facetable: ファセットを可能にする。フィールドの長さは最大32KB。Edm.GeographyPoint は利用不可。
            // key: id の field 。Edm.String 必須。
            // retrievable: 検索結果として値を返す。
            // 参考: https://learn.microsoft.com/ja-jp/azure/search/search-what-is-an-index#field-attributes

            // ※ Field 定義の型について
            // 基本的には id に SimpleField, vector の field に SearchField, 文字列の field には SearchableField、あとは良しなに使い分ける。
            // - SimpleField: 常に検索不能で、retrievable (取得可能)。フィルター・ファセット、スコアリングプロファイルで使用されるので、基本的に ID とかに使う。
            // - SearchField: 言語アナライザーが Default Lucene にセットされている以外のプロパティが null のフィールド。IsFilterable、IsSortable、IsFacetable は必要に応じて true をセットする。
            // - SearchableField: SearchFieldのヘルパーで、値は文字列必須。常に searchable (検索可能) で retrievable (取得可能)。他の属性はデフォルトで OFF 
            // 参考: https://learn.microsoft.com/ja-jp/azure/search/search-howto-dotnet-sdk#choosing-a-field-class
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