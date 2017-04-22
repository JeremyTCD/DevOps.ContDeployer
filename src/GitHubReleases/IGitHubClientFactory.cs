using Octokit;

namespace JeremyTCD.ContDeployer.Plugin.GitHubReleases
{
    public interface IGitHubClientFactory
    {
        IGitHubClient CreateClient(string apiKey);
    }
}