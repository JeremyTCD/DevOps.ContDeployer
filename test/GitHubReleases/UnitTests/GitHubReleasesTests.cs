using JeremyTCD.ContDeployer.PluginTools;
using Moq;
using Octokit;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GitHubReleases.Tests.UnitTests
{
    public class GitHubReleasesTests
    {
        private MockRepository _mockRepository { get; }

        public GitHubReleasesTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNullOrNotAGitHubReleasesOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns((IPluginOptions)null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitHubReleases(null, mockStepContext.Object, null));
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
            mockStepContext.Setup(o => o.Options).Returns(new GitHubReleasesOptions
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

            GitHubReleases adapter = new GitHubReleases(mockPipelineContext.Object, mockStepContext.Object, mockGitHubClientFactory.Object);

            // Act
            adapter.Run();

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
            mockStepContext.Setup(o => o.Options).Returns(new GitHubReleasesOptions
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

            GitHubReleases adapter = new GitHubReleases(mockPipelineContext.Object, mockStepContext.Object, mockGitHubClientFactory.Object);

            // Act
            adapter.Run();

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_DoesNotEditOrCreateGitHubReleasesOnDryRun()
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
            mockStepContext.Setup(o => o.Options).Returns(new GitHubReleasesOptions
            {
                Token = testToken,
                Owner = testOwner,
                Repository = testRepository,
                ModifiedReleases = new List<ModifiedRelease> { modifiedRelease },
                NewReleases = new List<NewRelease> { newRelease}
            });

            Mock<IGitHubClient> mockGitHubClient = _mockRepository.Create<IGitHubClient>();
            Mock<IGitHubClientFactory> mockGitHubClientFactory = _mockRepository.Create<IGitHubClientFactory>();
            mockGitHubClientFactory.Setup(g => g.CreateClient(testToken)).Returns(mockGitHubClient.Object);

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock.Get(mockPipelineContext.Object.SharedOptions).Setup(s => s.DryRun).Returns(true);

            GitHubReleases adapter = new GitHubReleases(mockPipelineContext.Object, mockStepContext.Object, mockGitHubClientFactory.Object);

            // Act
            adapter.Run();

            // Assert
            _mockRepository.VerifyAll();
            mockGitHubClient.
                Verify(g => g.Repository.Release.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewRelease>()), Times.Never);
            mockGitHubClient.
                Verify(g => g.Repository.Release.Edit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ReleaseUpdate>()), Times.Never);
        }
    }
}
