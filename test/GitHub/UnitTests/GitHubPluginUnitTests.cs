using JeremyTCD.ContDeployer.PluginTools;
using Moq;
using Octokit;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GitHub.Tests.UnitTests
{
    public class GitHubPluginUnitTests
    {
        private MockRepository _mockRepository { get; }

        public GitHubPluginUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Run_ThrowsExceptionIfPluginOptionsIsNotAGitHubPluginOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns((IPluginOptions)null);

            Mock<IGitHubClientFactory> mockGitHubClientFactory = _mockRepository.Create<IGitHubClientFactory>();

            GitHubPlugin plugin = new GitHubPlugin(mockGitHubClientFactory.Object);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => plugin.Run(null, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_CreatesGitHubReleaseIfGitReleasesOptionsContainsANewRelease()
        {
            // Arrange
            string testOwner = "testOwner";
            string testRepository = "testRepository";
            string testToken = "testToken";
            string testTagName = "1.0.0";

            NewRelease newRelease = new NewRelease(testTagName);
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(o => o.PluginOptions).Returns(new GitHubPluginOptions
            {
                Token = testToken,
                Owner = testOwner,
                Repository = testRepository,
                NewReleases = new List<NewRelease> { newRelease }
            });

            Mock<IGitHubClient> mockGitHubClient = _mockRepository.Create<IGitHubClient>();
            Mock.Get(mockGitHubClient.Object.Repository.Release).Setup(r => r.Create(testOwner, testRepository, newRelease));
            Mock<IGitHubClientFactory> mockGitHubClientFactory = _mockRepository.Create<IGitHubClientFactory>();
            mockGitHubClientFactory.Setup(g => g.CreateClient(testToken)).Returns(mockGitHubClient.Object);

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock.Get(mockPipelineContext.Object.SharedOptions).Setup(s => s.DryRun).Returns(false);

            GitHubPlugin plugin = new GitHubPlugin(mockGitHubClientFactory.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_EditsGitHubReleaseIfGitReleasesOptionsContainsAModifiedRelease()
        {
            // Arrange
            string testOwner = "testOwner";
            string testRepository = "testRepository";
            string testToken = "testToken";
            int testId = 1;

            ReleaseUpdate releaseUpdate = new ReleaseUpdate();
            ModifiedRelease modifiedRelease = new ModifiedRelease
            {
                Id = testId,
                ReleaseUpdate = releaseUpdate
            };
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(o => o.PluginOptions).Returns(new GitHubPluginOptions
            {
                Token = testToken,
                Owner = testOwner,
                Repository = testRepository,
                ModifiedReleases = new List<ModifiedRelease> { modifiedRelease }
            });

            Mock<IGitHubClient> mockGitHubClient = _mockRepository.Create<IGitHubClient>();
            Mock.Get(mockGitHubClient.Object.Repository.Release).Setup(r => r.Edit(testOwner, testRepository, testId, releaseUpdate));
            Mock<IGitHubClientFactory> mockGitHubClientFactory = _mockRepository.Create<IGitHubClientFactory>();
            mockGitHubClientFactory.Setup(g => g.CreateClient(testToken)).Returns(mockGitHubClient.Object);

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock.Get(mockPipelineContext.Object.SharedOptions).Setup(s => s.DryRun).Returns(false);

            GitHubPlugin plugin = new GitHubPlugin(mockGitHubClientFactory.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_DoesNotEditOrCreateGitHubOnDryRun()
        {
            // Arrange
            string testOwner = "testOwner";
            string testRepository = "testRepository";
            string testToken = "testToken";
            int testId = 1;
            string testTagName = "1.0.0";

            NewRelease newRelease = new NewRelease(testTagName);
            ReleaseUpdate releaseUpdate = new ReleaseUpdate();
            ModifiedRelease modifiedRelease = new ModifiedRelease
            {
                Id = testId,
                ReleaseUpdate = releaseUpdate
            };
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(o => o.PluginOptions).Returns(new GitHubPluginOptions
            {
                Token = testToken,
                Owner = testOwner,
                Repository = testRepository,
                ModifiedReleases = new List<ModifiedRelease> { modifiedRelease },
                NewReleases = new List<NewRelease> { newRelease }
            });

            Mock<IGitHubClient> mockGitHubClient = _mockRepository.Create<IGitHubClient>();
            Mock<IGitHubClientFactory> mockGitHubClientFactory = _mockRepository.Create<IGitHubClientFactory>();
            mockGitHubClientFactory.Setup(g => g.CreateClient(testToken)).Returns(mockGitHubClient.Object);

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock.Get(mockPipelineContext.Object.SharedOptions).Setup(s => s.DryRun).Returns(true);

            GitHubPlugin plugin = new GitHubPlugin(mockGitHubClientFactory.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            mockGitHubClient.
                Verify(g => g.Repository.Release.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewRelease>()), Times.Never);
            mockGitHubClient.
                Verify(g => g.Repository.Release.Edit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ReleaseUpdate>()), Times.Never);
        }
    }
}
