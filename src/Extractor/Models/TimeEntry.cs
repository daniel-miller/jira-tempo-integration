namespace Extractor;

public class TimeEntry
{
    public string Date { get; set; }

    public string Worker { get; set; }

    public int Seconds { get; set; }

    public string Account { get; set; }

    public string Type { get; set; }

    public string Issue { get; set; }

    public bool Subtask { get; set; }

    public string Labels { get; set; }

    public string Summary { get; set; }

    public int Minutes => (int)Math.Round((decimal)Seconds / 60, 2);

    public decimal Hours => Math.Round((decimal)Minutes / 60, 2);

    public override string ToString()
    {
        var summary = Summary;

        if (summary != null)
            summary = summary.Replace(",", ";").Replace("\n", " -- ").Replace("\r", " -- ");

        return $"{Date},{Worker},{Hours:n2},{Account},{Type},{Issue},{Subtask},{Labels},{Summary}\n";
    }
}
