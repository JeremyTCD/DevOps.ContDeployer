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

        public static Release CreateRelease(string tagName, string body)
        {
            Release release = new StubRelease(tagName, body);
            return release;
        }

        public static List<Release> CreateReleases(Release mockRelease = null)
        {
            List<Release> releases = new List<Release>();
            if(mockRelease != null)
            {
                releases.Add(mockRelease);
            }
            return releases;
        }
    }

    /// <summary>
    /// <see cref="Release"/>s properties are not overridable. Though setters are protected, so this will
    /// do.
    /// </summary>
    public class StubRelease : Release
    {
        public StubRelease(string tagName, string body) : base()
        {
            TagName = tagName;
            Body = body;
        }
    }
}
