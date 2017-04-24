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
            IStepContext stepContext = PluginTestHelpers.CreateStepContext(null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitHubReleasesChangelogAdapter(null, stepContext, null));
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new GitHubReleasesChangelogAdapterOptions());
            IPipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitHubReleasesChangelogAdapter(pipelineContext, stepContext, null));
        }

        [Fact]
        public void Run_AddsGitHubReleasesStepWithOptionsContainingANewReleaseIfAVersionHasNoCorrespondingRelease()
        {
            // Arrange
            string testOwner = "testOwner";
            string testRepository = "testRepository";
            string testToken = "testToken";
            string testVersion = "1.0.0";

            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new GitHubReleasesChangelogAdapterOptions
            {
                Token = testToken,
                Owner = testOwner,
                Repository = testRepository
            });

            IReleasesClient mockReleaseClient = GitHubReleasesTestHelpers.CreateReleaseClient(testOwner, testRepository, new List<Release>());
            IRepositoriesClient mockRepositoriesClient = GitHubReleasesTestHelpers.CreateRepositoriesClient(mockReleaseClient);
            IGitHubClient mockGitHubClient = GitHubReleasesTestHelpers.CreateGitHubClient(mockRepositoriesClient);
            IGitHubClientFactory mockGitHubClientFactory = GitHubReleasesTestHelpers.CreateGitHubClientFactory(testToken, mockGitHubClient);

            SortedSet<IVersion> versions = new SortedSet<IVersion>()
            {
                new ChangelogGenerator.Version { SemVersion = SemVersion.Parse(testVersion) }
            };
            IChangelog mockChangelog = ChangelogGeneratorTestHelpers.CreateChangelog(versions);
            IDictionary<string, object> mockSharedData = PluginTestHelpers.CreateSharedData(nameof(Changelog), mockChangelog);
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreatePipelineContext(mockSharedData);

            GitHubReleasesChangelogAdapter adapter = new GitHubReleasesChangelogAdapter(mockPipelineContext, stepContext, mockGitHubClientFactory);

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
