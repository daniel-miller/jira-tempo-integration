using Newtonsoft.Json;

namespace Extractor;

public class Worklog
{
    [JsonProperty("attributes")]
    public object Attributes { get; set; } = null!;

    [JsonProperty("author")]
    public object Author { get; set; } = null!;

    [JsonProperty("billableSeconds")]
    public int BillableSeconds { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; } = null!;

    [JsonProperty("issue")]
    public WorklogIssue Issue { get; set; } = null!;

    [JsonProperty("self")]
    public string Self { get; set; } = null!;

    [JsonProperty("startDate")]
    public string StartDate { get; set; } = null!;

    [JsonProperty("startTime")]
    public string StartTime { get; set; } = null!;

    [JsonProperty("tempoWorklogId")]
    public int TempoWorklogId { get; set; }

    [JsonProperty("timeSpentSeconds")]
    public int TimeSpentSeconds { get; set; }

    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
