namespace Extractor;

public class ExtractorApp
{
    private readonly ExtractorSettings _settings;

    public ExtractorApp(ExtractorSettings settings)
    {
        _settings = settings;
    }

    public async Task Run()
    {
        var jira = new JiraClient(_settings);

        var tempo = new TempoClient(_settings, jira);

        var entries = await tempo.GetTimeEntries(_settings.Since, _settings.Until);

        var list = entries.OrderBy(x => x.Date)
            .ThenBy(x => x.Worker)
            .ThenBy(x => x.Account)
            .ThenBy(x => x.Type)
            .ThenBy(x => x.Issue)
            .ToList();

        BuildReports(list);
    }

    private void BuildReports(List<TimeEntry> list)
    {
        var detail = new DetailReport(list);

        File.WriteAllText(_settings.DetailReportPath, detail.Text);

        var summary = new SummaryReport(list);

        File.WriteAllText(_settings.SummaryReportPath, summary.Text);
    }
}
