using System.Net.Http.Headers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog;

namespace Extractor;

public class TempoClient
{
    public string? NextUrl { get; set; }

    private readonly ExtractorSettings _settings;

    private readonly IJiraClient _jira;

    private readonly Dictionary<string, Issue> _issues = [];

    private readonly Dictionary<string, Worker> _workers = [];

    public TempoClient(ExtractorSettings settings, IJiraClient jira)
    {
        _settings = settings;

        _jira = jira;
    }

    private HttpClient CreateHttpClient()
    {
        var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.TempoApiToken);

        return client;
    }

    public async Task<TimeEntry[]> GetTimeEntries(DateTime since, DateTime until)
    {
        var list = new List<TimeEntry>();

        NextUrl = $"{_settings.TempoBaseUrl}/4/worklogs?from={since:yyyy-MM-dd}&to={until:yyyy-MM-dd}";

        while (!string.IsNullOrEmpty(NextUrl))
        {
            list.AddRange(await GetTimeEntryBatch());
        }

        return list.ToArray();
    }

    private async Task<TimeEntry[]> GetTimeEntryBatch()
    {
        Log.Information("Getting worklogs {url}", NextUrl);

        var list = new List<TimeEntry>();

        using (var client = CreateHttpClient())
        {
            var tempoResponse = await client.GetAsync(NextUrl);

            if (tempoResponse.IsSuccessStatusCode)
            {
                var response = await ParseResponse(tempoResponse);

                Log.Information("  {worklogs:n0} Worklogs", response.Results.Count());

                foreach (var worklog in response.Results)
                {
                    var entry = new TimeEntry();

                    var issueSelf = worklog.Issue.Self;

                    // Avoid asking the Jira API for the same issue more than once.

                    if (!_issues.ContainsKey(issueSelf))
                    {
                        var newIssue = await _jira.GetIssue(issueSelf);

                        _issues.Add(issueSelf, newIssue);
                    }

                    var issue = _issues[issueSelf];

                    var accountId = ParseAuthorAccountId(worklog);

                    var accountUrl = ParseAuthorAccountUrl(worklog);

                    if (accountId != null)
                    {
                        // Avoid asking the Jira API for the same worker more than once.

                        if (!_workers.ContainsKey(accountUrl))
                        {
                            var newWorker = await _jira.GetWorker(accountUrl);

                            _workers.Add(accountUrl, newWorker);
                        }

                        var worker = _workers[accountUrl];

                        entry.Worker = worker.Name;
                    }

                    entry.Date = worklog.StartDate;

                    entry.Seconds = worklog.TimeSpentSeconds;

                    entry.Account = issue.Account;

                    entry.Type = issue.Type;

                    entry.Issue = issue.Number;

                    entry.Subtask = issue.Subtask;

                    entry.Labels = issue.Labels;

                    entry.Summary = issue.Summary;

                    list.Add(entry);
                }

                // Get the next URL from metadata.

                NextUrl = response.Next;
            }
            else
            {
                Log.Error("Error retrieving worklogs: " + tempoResponse.ReasonPhrase);

                NextUrl = null;
            }
        }

        return list.ToArray();
    }

    private async Task<WorklogsResponse> ParseResponse(HttpResponseMessage response)
    {
        var worklogResponse = new WorklogsResponse();

        var jsonString = await response.Content.ReadAsStringAsync();

        var jsonResponse = JObject.Parse(jsonString);

        worklogResponse.Results = JsonConvert.DeserializeObject<Worklog[]>(jsonResponse["results"].ToString());

        worklogResponse.Next = jsonResponse["metadata"]?["next"]?.ToString();

        return worklogResponse;
    }

    private string ParseAuthorAccountId(Worklog worklog)
        => ((JValue)((JObject)worklog.Author)["accountId"]).Value as string;

    private string ParseAuthorAccountUrl(Worklog worklog)
        => ((JValue)((JObject)worklog.Author)["self"]).Value as string;
}
