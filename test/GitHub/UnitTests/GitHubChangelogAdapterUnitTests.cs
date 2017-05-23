using JeremyTCD.PipelinesCE.Plugin.Changelog;
using JeremyTCD.PipelinesCE.PluginTools;
using Moq;
using Octokit;
using Semver;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.Plugin.GitHub.Tests.UnitTests
{
    public class GitHubChangelogAdapterTests
    {
        private MockRepository _mockRepository { get; }

        public GitHubChangelogAdapterTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Run_ThrowsExceptionIfPluginOptionsIsNotAGitHubChangelogAdapterOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns((IPluginOptions)null);

            Mock<IGitHubClientFactory> mockGitHubClientFactory = _mockRepository.Create<IGitHubClientFactory>();

            GitHubChangelogAdapter adapter = new GitHubChangelogAdapter(mockGitHubClientFactory.Object);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => adapter.Run(null, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns(new GitHubChangelogAdapterOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = null;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));

            GitHubChangelogAdapter adapter = new GitHubChangelogAdapter(null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => adapter.Run(mockPipelineContext.Object, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_AddsGitHubPluginStepWithOptionsContainingANewReleaseIfAVersionHasNoCorrespondingRelease()
        {
            // Arrange
            string testOwner = "testOwner";
            string testRepository = "testRepository";
            string testToken = "testToken";
            string testVersion = "1.0.0";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(o => o.PluginOptions).Returns(new GitHubChangelogAdapterOptions
            {
                Token = testToken,
                Owner = testOwner,
                Repository = testRepository
            });

            Mock<IGitHubClient> mockGitHubClient = _mockRepository.Create<IGitHubClient>();
            Mock<IReleasesClient> mockReleasesClient = Mock.Get(mockGitHubClient.Object.Repository.Release);
            mockReleasesClient.Setup(r => r.GetAll(testOwner, testRepository)).ReturnsAsync(new List<Release>());
            Mock<IGitHubClientFactory> mockGitHubClientFactory = _mockRepository.Create<IGitHubClientFactory>();
            mockGitHubClientFactory.Setup(g => g.CreateClient(testToken)).Returns(mockGitHubClient.Object);

            Mock<IVersion> mockVersion = _mockRepository.Create<IVersion>();
            mockVersion.Setup(v => v.SemVersion).Returns(SemVersion.Parse(testVersion));
            SortedSet<IVersion> versions = new SortedSet<IVersion> { mockVersion.Object };
            Mock<IChangelog> mockChangelog = _mockRepository.Create<IChangelog>();
            mockChangelog.Setup(c => c.Versions).Returns(versions);
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = mockChangelog.Object;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));

            LinkedList<IStep> remainingSteps = new LinkedList<IStep>();
            mockStepContext.Setup(o => o.RemainingSteps).Returns(remainingSteps);

            GitHubChangelogAdapter adapter = new GitHubChangelogAdapter(mockGitHubClientFactory.Object);  

            // Act
            adapter.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            Assert.Equal(1, remainingSteps.Count);
            GitHubPluginOptions gitHubPluginOptions = remainingSteps.First.Value.PluginOptions as GitHubPluginOptions;
            Assert.NotNull(gitHubPluginOptions);
            Assert.Equal(1, gitHubPluginOptions.NewReleases.Count);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_AddsGitHubStepWithOptionsContainingAReleaseUpdateIfAVersionsNotesAreInconsistentWithItsReleases()
        {
            // Arrange
            string testOwner = "testOwner";
            string testRepository = "testRepository";
            string testToken = "testToken";
            string testVersion = "1.0.0";
            string testBody1 = "testBody1";
            string testBody2 = "testBody2";
            int testId = 1;

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(o => o.PluginOptions).Returns(new GitHubChangelogAdapterOptions
            {
                Token = testToken,
                Owner = testOwner,
                Repository = testRepository
            });

            Mock<IGitHubClient> mockGitHubClient = _mockRepository.Create<IGitHubClient>();
            Release release = new StubRelease(testVersion, testBody1, testId);
            Mock<IReleasesClient> mockReleasesClient = Mock.Get(mockGitHubClient.Object.Repository.Release);
            mockReleasesClient.Setup(r => r.GetAll(testOwner, testRepository)).ReturnsAsync(new List<Release> { release });
            Mock<IGitHubClientFactory> mockGitHubClientFactory = _mockRepository.Create<IGitHubClientFactory>();
            mockGitHubClientFactory.Setup(g => g.CreateClient(testToken)).Returns(mockGitHubClient.Object);

            Mock<IVersion> mockVersion = _mockRepository.Create<IVersion>();
            mockVersion.Setup(v => v.SemVersion).Returns(SemVersion.Parse(testVersion));
            mockVersion.Setup(v => v.Notes).Returns(testBody2);
            SortedSet<IVersion> versions = new SortedSet<IVersion>();
            versions.Add(mockVersion.Object);
            Mock<IChangelog> mockChangelog = _mockRepository.Create<IChangelog>();
            mockChangelog.Setup(c => c.Versions).Returns(versions);
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = mockChangelog.Object;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));
            Mock<IStep> mockStep = _mockRepository.Create<IStep>();

            LinkedList<IStep> remainingSteps = new LinkedList<IStep>();
            mockStepContext.Setup(o => o.RemainingSteps).Returns(remainingSteps);

            GitHubChangelogAdapter adapter = new GitHubChangelogAdapter(mockGitHubClientFactory.Object);

            // Act
            adapter.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            Assert.Equal(1, remainingSteps.Count);
            GitHubPluginOptions gitHubPluginOptions = remainingSteps.First.Value.PluginOptions as GitHubPluginOptions;
            Assert.NotNull(gitHubPluginOptions);
            Assert.Equal(1, gitHubPluginOptions.ModifiedReleases.Count);
            _mockRepository.VerifyAll();
        }

        /// <summary>
        /// <see cref="Release"/>s properties are not overridable. Though setters are protected, so this will
        /// do.
        /// </summary>
        private class StubRelease : Release
        {
            public StubRelease(string tagName, string body, int id) : base()
            {
                Name = tagName;
                Body = body;
                Id = id;
            }
        }
    }
}
