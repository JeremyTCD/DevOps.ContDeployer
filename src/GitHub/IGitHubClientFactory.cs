using Octokit;

namespace JeremyTCD.ContDeployer.Plugin.GitHub
{
    public interface IGitHubClientFactory
    {
        IGitHubClient CreateClient(string apiKey);
    }
}