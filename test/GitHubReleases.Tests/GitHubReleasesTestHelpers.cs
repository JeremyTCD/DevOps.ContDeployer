using NSubstitute;
using Octokit;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.GitHubReleases.Tests
{
    public class GitHubReleasesTestHelpers
    {
        public static IGitHubClientFactory CreateMockGitHubClientFactory(string token, IGitHubClient gitHubClient)
        {
            IGitHubClientFactory mockGitHubClientFactory = Substitute.For<IGitHubClientFactory>();
            mockGitHubClientFactory.CreateClient(token).Returns(gitHubClient);

            return mockGitHubClientFactory;
        }

        public static IGitHubClient CreateMockGitHubClient(IRepositoriesClient repositoriesClient)
        {
            IGitHubClient mockGitHubClient = Substitute.For<IGitHubClient>();
            mockGitHubClient.Repository.Returns(repositoriesClient);

            return mockGitHubClient;
        }

        public static IRepositoriesClient CreateMockRepositoriesClient(IReleasesClient releaseClient)
        {
            IRepositoriesClient mockRepositoriesClient = Substitute.For<IRepositoriesClient>();
            mockRepositoriesClient.Release.Returns(releaseClient);

            return mockRepositoriesClient;
        }

        public static IReleasesClient CreateMockReleaseClient(string owner, string repository, List<Release> releases)
        {
            IReleasesClient mockReleaseClient = Substitute.For<IReleasesClient>();
            mockReleaseClient.GetAll(owner, repository).Returns(releases);

            return mockReleaseClient;
        }
    }
}
