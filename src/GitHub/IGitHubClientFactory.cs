using Octokit;

namespace JeremyTCD.PipelinesCE.Plugin.GitHub
{
    public interface IGitHubClientFactory
    {
        IGitHubClient CreateClient(string apiKey);
    }
}