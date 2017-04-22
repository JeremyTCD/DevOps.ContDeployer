using Octokit;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleases
{
    public interface IGithubClientFactory
    {
        IGitHubClient CreateClient(string apiKey);
    }
}