using System.Collections.Generic;

namespace Indexer;

internal class AzureDoc
{
    public string Id { get; set; }
    public string Category { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public IReadOnlyList<float> ContentVector { get; set; }
}