using System.Text;

using Serilog;

namespace Extractor;

public class SummaryReport
{
    public string Text { get; set; }

    public SummaryReport(List<TimeEntry> entries)
    {
        var sum = entries.Sum(x => x.Hours);

        Log.Information("{hours:n2} total hours logged in this time period.", sum);

        var sb = new StringBuilder();

        sb.AppendLine("## Hours per Account");

        sb.AppendLine();

        sb.AppendLine($"Account,Hours");

        var accounts = entries.GroupBy(x => x.Account).OrderBy(x => x.Key);

        foreach (var account in accounts)
        {
            var accountSum = account.Sum(x => x.Hours);

            sb.AppendLine($"{account.Key},{accountSum:n2}");
        }

        sb.AppendLine();

        sb.AppendLine("## Hours per Worker");

        sb.AppendLine();

        sb.AppendLine($"Worker,Hours");

        var workers = entries.GroupBy(x => x.Worker).OrderBy(x => x.Key);

        foreach (var worker in workers)
        {
            var workerSum = worker.Sum(x => x.Hours);

            sb.AppendLine($"{worker.Key},{workerSum:n2}");
        }

        Text = sb.ToString();
    }
}