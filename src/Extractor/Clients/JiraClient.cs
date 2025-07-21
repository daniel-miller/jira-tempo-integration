using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json.Linq;

using Serilog;

namespace Extractor;

public class JiraClient : IJiraClient
{
    private readonly ExtractorSettings _settings;

    public JiraClient(ExtractorSettings settings)
    {
        _settings = settings;
    }

    private HttpClient CreateHttpClient()
    {
        var email = _settings.JiraUserEmail;

        var token = _settings.JiraApiToken;

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{token}"));

        var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        return client;
    }

    public async Task<Issue> GetIssue(string issueUrl)
    {
        var issue = new Issue { Url = issueUrl };

        using (var client = CreateHttpClient())
        {
            Log.Information("  Getting issue {issue}", issue.Url);

            var jiraResponse = await client.GetAsync(issue.Url);

            if (jiraResponse.IsSuccessStatusCode)
            {
                var jsonString = await jiraResponse.Content.ReadAsStringAsync();

                var jsonResponse = JObject.Parse(jsonString);

                issue.Number = jsonResponse["key"].ToString();

                issue.Summary = jsonResponse["fields"]?["summary"]?.ToString();

                var customfield = jsonResponse["fields"]?["customfield_10033"];

                if (customfield != null && customfield.HasValues)
                    issue.Account = customfield["value"]?.ToString();

                // If the Account is blank then look for an account on the parent issue.
                if (string.IsNullOrEmpty(issue.Account))
                    issue.Account = jsonResponse["fields"]?["parent"]?["fields"]?["customfield_10033"]?["value"]?.ToString();

                issue.Type = jsonResponse["fields"]?["issuetype"]?["name"]?.ToString();

                issue.Subtask = jsonResponse["fields"]?["issuetype"]?["subtask"]?.ToString() == "True";

                if (issue.Subtask)
                    issue.Type = jsonResponse["fields"]?["parent"]?["fields"]?["issuetype"]?["name"]?.ToString();

                JArray? jsonArray = jsonResponse["fields"]?["labels"] as JArray;

                if (jsonArray != null)
                {
                    // Omit labels named OutScope and InScope. I don't need these labels in my report.

                    var array = jsonArray.Select(token => token.ToString())
                        .Where(x => x != "OutScope" && x != "InScope")
                        .ToArray();

                    if (array != null && array.Length > 0)
                        issue.Labels = string.Join("|", array);
                }

                Log.Information("    {issue} {account} : {summary} ", issue.Number, issue.Account, issue.Summary);
            }
            else
            {
                Log.Error($"Error retrieving issue: {jiraResponse.ReasonPhrase}");
            }
        }

        return issue;
    }

    public async Task<Worker> GetWorker(string workerUrl)
    {
        var worker = new Worker { Url = workerUrl };

        using (var client = CreateHttpClient())
        {
            Log.Information("  Getting worker {worker}", workerUrl);

            var response = await client.GetAsync(workerUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                var jsonResponse = JObject.Parse(jsonString);

                worker.Name = jsonResponse["displayName"].ToString();

                Log.Information("    {worker}", worker.Name);
            }
            else
            {
                Log.Error($"Error retrieving issue: {response.ReasonPhrase}");
            }
        }

        return worker;
    }
}