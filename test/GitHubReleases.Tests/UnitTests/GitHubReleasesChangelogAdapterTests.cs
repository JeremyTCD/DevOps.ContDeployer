using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.Tests;
using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using Octokit;
using Semver;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GitHubReleases.Tests.UnitTests
{
    public class GitHubReleasesChangelogAdapterTests
    {
        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            IStepContext mockStepContext = PluginTestHelpers.CreateMockStepContext();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitHubReleasesChangelogAdapter(null, mockStepContext, null));
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            IStepContext mockStepContext = PluginTestHelpers.CreateMockStepContext(new GitHubReleasesChangelogAdapterOptions());
            IDictionary<string, object> sharedData = PluginTestHelpers.CreateSharedData();
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreateMockPipelineContext(sharedData);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitHubReleasesChangelogAdapter(mockPipelineContext, mockStepContext, null));
        }

        [Fact]
        public void Run_AddsGitHubReleasesStepWithOptionsContainingANewReleaseIfAVersionHasNoCorrespondingRelease()
        {
            // Arrange
            string testOwner = "testOwner";
            string testRepository = "testRepository";
            string testToken = "testToken";
            string testVersion = "1.0.0";

            IStepContext mockStepContext = PluginTestHelpers.CreateMockStepContext(new GitHubReleasesChangelogAdapterOptions
            {
                Token = testToken,
                Owner = testOwner,
                Repository = testRepository
            });

            IReleasesClient mockReleaseClient = GitHubReleasesTestHelpers.CreateMockReleaseClient(testOwner, testRepository, new List<Release>());
            IRepositoriesClient mockRepositoriesClient = GitHubReleasesTestHelpers.CreateMockRepositoriesClient(mockReleaseClient);
            IGitHubClient mockGitHubClient = GitHubReleasesTestHelpers.CreateMockGitHubClient(mockRepositoriesClient);
            IGitHubClientFactory mockGitHubClientFactory = GitHubReleasesTestHelpers.CreateMockGitHubClientFactory(testToken, mockGitHubClient);

            SortedSet<IVersion> versions = new SortedSet<IVersion>()
            {
                new ChangelogGenerator.Version { SemVersion = SemVersion.Parse(testVersion) }
            };
            IChangelog mockChangelog = ChangelogGeneratorTestHelpers.CreateMockChangelog(versions);
            IDictionary<string, object> sharedData = PluginTestHelpers.CreateSharedData(nameof(Changelog), mockChangelog);
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreateMockPipelineContext(sharedData, stepFactory: mockStepFactory, 
                steps: steps);

            GitHubReleasesChangelogAdapter adapter = new GitHubReleasesChangelogAdapter(mockPipelineContext, mockStepContext, mockGitHubClientFactory);

            // Act
            adapter.Run();

            // Assert
            Assert.Equal(1, mockPipelineContext.Steps.Count);
            IStep step = mockPipelineContext.Steps.First.Value;
            Assert.Equal(nameof(GitHubReleases), step.PluginName);
            GitHubReleasesOptions options = step.Options as GitHubReleasesOptions;
            Assert.NotNull(options);
            Assert.Equal(1, options.NewReleases.Count);
            Assert.Equal(testVersion, options.NewReleases[0].Name);
        }

        [Fact]
        public void Run_AddsGitHubReleasesStepWithReleaseUpdateIfAVersionsNotesAreInconsistentWithItsReleases()
        {

        }
    }
}
