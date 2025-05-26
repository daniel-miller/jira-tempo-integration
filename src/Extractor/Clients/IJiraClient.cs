
namespace Extractor
{
    public interface IJiraClient
    {
        Task<Issue> GetIssue(string issueUrl);
        Task<Worker> GetWorker(string workerUrl);
    }
}