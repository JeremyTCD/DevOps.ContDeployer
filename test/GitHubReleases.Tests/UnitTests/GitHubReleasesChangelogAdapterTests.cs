using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.Tests;
using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using NSubstitute;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
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

            List<Release> releases = GitHubReleasesTestHelpers.CreateReleases();
            IReleasesClient mockReleaseClient = GitHubReleasesTestHelpers.CreateMockReleaseClient(testOwner, testRepository, 
                releases);
            IRepositoriesClient mockRepositoriesClient = GitHubReleasesTestHelpers.CreateMockRepositoriesClient(mockReleaseClient);
            IGitHubClient mockGitHubClient = GitHubReleasesTestHelpers.CreateMockGitHubClient(mockRepositoriesClient);
            IGitHubClientFactory mockGitHubClientFactory = GitHubReleasesTestHelpers.CreateMockGitHubClientFactory(testToken, mockGitHubClient);

            IStep mockStep = PluginTestHelpers.CreateMockStep(nameof(GitHubReleases));
            IStepFactory mockStepFactory = PluginTestHelpers.CreateMockStepFactory(nameof(GitHubReleases), mockStep);
            LinkedList<IStep> steps = PluginTestHelpers.CreateSteps();
            SortedSet<IVersion> versions = ChangelogGeneratorTestHelpers.CreateVersions(testVersion);
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
            GitHubReleasesOptions generatedOptions = mockStepFactory.ReceivedCalls().First().GetArguments()[1] as GitHubReleasesOptions;
            Assert.NotNull(generatedOptions);
            Assert.Equal(1, generatedOptions.NewReleases.Count);
            Assert.Equal(testVersion, generatedOptions.NewReleases[0].Name);
        }

        [Fact]
        public void Run_AddsGitHubReleasesStepWithOptionsContainingAReleaseUpdateIfAVersionsNotesAreInconsistentWithItsReleases()
        {
            // Arrange
            string testOwner = "testOwner";                 
            string testRepository = "testRepository";
            string testToken = "testToken";
            string testVersion = "1.0.0";
            string testBody1 = "testBody1";
            string testBody2 = "testBody2";

            IStepContext mockStepContext = PluginTestHelpers.CreateMockStepContext(new GitHubReleasesChangelogAdapterOptions
            {
                Token = testToken,
                Owner = testOwner,
                Repository = testRepository
            });

            // create mock release
            Release release = GitHubReleasesTestHelpers.CreateRelease(testVersion, testBody1);
            List<Release> releases = GitHubReleasesTestHelpers.CreateReleases(release);
            IReleasesClient mockReleaseClient = GitHubReleasesTestHelpers.CreateMockReleaseClient(testOwner, testRepository, 
                releases);
            IRepositoriesClient mockRepositoriesClient = GitHubReleasesTestHelpers.CreateMockRepositoriesClient(mockReleaseClient);
            IGitHubClient mockGitHubClient = GitHubReleasesTestHelpers.CreateMockGitHubClient(mockRepositoriesClient);
            IGitHubClientFactory mockGitHubClientFactory = GitHubReleasesTestHelpers.CreateMockGitHubClientFactory(testToken, mockGitHubClient);

            IStep mockStep = PluginTestHelpers.CreateMockStep(nameof(GitHubReleases));
            IStepFactory mockStepFactory = PluginTestHelpers.CreateMockStepFactory(nameof(GitHubReleases), mockStep);
            LinkedList<IStep> steps = PluginTestHelpers.CreateSteps();
            SortedSet<IVersion> versions = ChangelogGeneratorTestHelpers.CreateVersions(testVersion, testBody2);
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
            GitHubReleasesOptions generatedOptions = mockStepFactory.ReceivedCalls().First().GetArguments()[1] as GitHubReleasesOptions;
            Assert.NotNull(generatedOptions);
            Assert.Equal(1, generatedOptions.ReleaseUpdates.Count);
            Assert.Equal(testVersion, generatedOptions.ReleaseUpdates[0].Name);
            Assert.Equal(testBody2, generatedOptions.ReleaseUpdates[0].Body);
        }
    }
}
