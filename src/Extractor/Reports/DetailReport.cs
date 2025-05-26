using System.Text;

namespace Extractor;

public class DetailReport
{
    public string Text { get; set; }

    public DetailReport(List<TimeEntry> entries)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Date,Worker,Hours,Account,Type,Issue,Subtask,Labels,Summary\n");

        foreach (var entry in entries)
            sb.AppendLine(entry.ToString());

        Text = sb.ToString();
    }
}