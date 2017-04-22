using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using LibGit2Sharp;
using Moq;
using NSubstitute;
using Octokit;
using Semver;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleases.IntegrationTests
{
    [Collection(nameof(GithubReleasesCollection))]
    public class GithubReleasesChangelogAdapterTests
    {
        private LibGit2Sharp.Signature _signature { get; }

        public GithubReleasesChangelogAdapterTests(GithubReleasesFixture fixture)
        {
            fixture.ResetTempDir();
            _signature = fixture.Signature;
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            StepContext stepContext = PluginTestHelpers.CreateStepContext();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GithubReleasesChangelogAdapter(null, stepContext, null));
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            PipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();
            StepContext stepContext = PluginTestHelpers.CreateStepContext(new GithubReleasesChangelogAdapterOptions());

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GithubReleasesChangelogAdapter(pipelineContext, stepContext, null));
        }

        [Fact]
        public void Run_AddsGithubReleasesStepWithOptionsContainingANewReleaseIfAVersionHasNoCorrespondingRelease()
        {
            // Arrange
            string testOwner = "testOwner";
            string testRepository = "testRepository";
            string testToken = "testToken";

            StepContext stepContext = PluginTestHelpers.CreateStepContext(new GithubReleasesChangelogAdapterOptions
            {
                Token = testToken
            });
            PipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();
            string testVersion = "1.0.0";
            SortedSet<ChangelogGenerator.Version> versions = new SortedSet<ChangelogGenerator.Version>()
            {
                new ChangelogGenerator.Version { SemVersion = SemVersion.Parse(testVersion) }
            };
            Changelog changelog = new Changelog(versions);
            pipelineContext.SharedData.Add(nameof(Changelog), changelog);

            // TODO move these to their own function, will be used in other functions
            IReleasesClient mockReleaseClient = Substitute.For<IReleasesClient>();
            mockReleaseClient.GetAll(testOwner, testRepository).Returns(new List<Release>());

            IRepositoriesClient mockRepositoriesClient = Substitute.For<IRepositoriesClient>();
            mockRepositoriesClient.Release.Returns(mockReleaseClient);

            IGitHubClient mockGithubClient = Substitute.For<IGitHubClient>();
            mockGithubClient.Repository.Returns(mockRepositoriesClient);

            IGithubClientFactory mockGithubClientFactory = Substitute.For<IGithubClientFactory>();
            mockGithubClientFactory.CreateClient(testToken).Returns(mockGithubClient);

            GithubReleasesChangelogAdapter adapter = new GithubReleasesChangelogAdapter(pipelineContext, stepContext, mockGithubClientFactory);

            // Act
            adapter.Run();

            // Assert
            Assert.Equal(1, pipelineContext.Steps.Count);
            Step step = pipelineContext.Steps.First.Value;
            Assert.Equal(nameof(GithubReleases), step.PluginName);
            GithubReleasesOptions options = step.Options as GithubReleasesOptions;
            Assert.NotNull(options);
            Assert.Equal(1, options.NewReleases.Count);
            Assert.Equal(testVersion, options.NewReleases[0].Name);
        }

        [Fact]
        public void Run_AddsGithubReleasesStepWithReleaseUpdateIfAVersionsNotesAreInconsistentWithItsReleases()
        {

        }
    }
}
