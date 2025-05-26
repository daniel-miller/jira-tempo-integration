using Newtonsoft.Json;

namespace Extractor;

public class WorklogIssue
{
    [JsonProperty("self")]
    public string Self { get; set; } = null!;
}
