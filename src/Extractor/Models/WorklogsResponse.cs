namespace Extractor;

public class WorklogsResponse
{
    public Worklog[] Results { get; set; } = null!;

    public string Next { get; set; } = null!;
}