using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using Moq;
using Octokit;
using Semver;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GitHubReleases.Tests.UnitTests
{
    public class GitHubReleasesChangelogAdapterTests
    {
        private MockRepository _mockRepository { get; }

        public GitHubReleasesChangelogAdapterTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNullOrNotAGitHubReleasesChangelogAdapterOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns((IPluginOptions)null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitHubReleasesChangelogAdapter(null, mockStepContext.Object, null));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns(new GitHubReleasesChangelogAdapterOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = null;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitHubReleasesChangelogAdapter(mockPipelineContext.Object, 
                mockStepContext.Object, null));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_AddsGitHubReleasesStepWithOptionsContainingANewReleaseIfAVersionHasNoCorrespondingRelease()
        {
            // Arrange
            string testOwner = "testOwner";
            string testRepository = "testRepository";
            string testToken = "testToken";
            string testVersion = "1.0.0";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(o => o.Options).Returns(new GitHubReleasesChangelogAdapterOptions
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
            SortedSet<IVersion> versions = new SortedSet<IVersion>();
            versions.Add(mockVersion.Object);
            Mock<IChangelog> mockChangelog = _mockRepository.Create<IChangelog>();
            mockChangelog.Setup(c => c.Versions).Returns(versions);
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = mockChangelog.Object;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));
            Mock<IStep> mockStep = _mockRepository.Create<IStep>();
            Mock<IStepFactory> mockStepFactory = Mock.Get(mockPipelineContext.Object.StepFactory);
            mockStepFactory.
                Setup(s => s.Build(nameof(GitHubReleases), 
                    It.Is<GitHubReleasesOptions>(o => o.NewReleases.Count == 1 && o.NewReleases[0].Name == testVersion))).
                Returns(mockStep.Object);

            LinkedList<IStep> steps = new LinkedList<IStep>();
            mockPipelineContext.Setup(o => o.Steps).Returns(steps);

            GitHubReleasesChangelogAdapter adapter = new GitHubReleasesChangelogAdapter(mockPipelineContext.Object, 
                mockStepContext.Object, mockGitHubClientFactory.Object);

            // Act
            adapter.Run();

            // Assert
            Assert.Equal(1, steps.Count);
            Assert.Equal(mockStep.Object, steps.First.Value);
            _mockRepository.VerifyAll();
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
            int testId = 1;

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(o => o.Options).Returns(new GitHubReleasesChangelogAdapterOptions
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
            Mock<IStepFactory> mockStepFactory = Mock.Get(mockPipelineContext.Object.StepFactory);
            mockStepFactory.
                Setup(s => s.Build(nameof(GitHubReleases),
                    It.Is<GitHubReleasesOptions>(o => o.ModifiedReleases.Count == 1 && 
                        o.ModifiedReleases[0].ReleaseUpdate.Name == testVersion &&
                        o.ModifiedReleases[0].ReleaseUpdate.Body == testBody2 && 
                        o.ModifiedReleases[0].Id == testId))).
                Returns(mockStep.Object);

            LinkedList<IStep> steps = new LinkedList<IStep>();
            mockPipelineContext.Setup(o => o.Steps).Returns(steps);

            GitHubReleasesChangelogAdapter adapter = new GitHubReleasesChangelogAdapter(mockPipelineContext.Object,
                mockStepContext.Object, mockGitHubClientFactory.Object);

            // Act
            adapter.Run();

            // Assert
            Assert.Equal(1, steps.Count);
            Assert.Equal(mockStep.Object, steps.First.Value);
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
