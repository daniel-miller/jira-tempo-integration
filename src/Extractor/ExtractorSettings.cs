namespace Extractor;

public class ExtractorSettings
{
    public string JiraUserEmail { get; set; } = null!;
    public string JiraBaseUrl { get; set; } = null!;
    public string JiraApiToken { get; set; } = null!;

    public string TempoBaseUrl { get; set; } = null!;
    public string TempoApiToken { get; set; } = null!;

    public DateTime Since { get; set; }
    public DateTime Until { get; set; }

    public string DetailReportPath { get; set; } = null!;
    public string SummaryReportPath { get; set; } = null!;
}
