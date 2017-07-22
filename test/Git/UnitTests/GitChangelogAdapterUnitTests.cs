using JeremyTCD.PipelinesCE.Plugin.Changelog;
using JeremyTCD.PipelinesCE.Core;
using LibGit2Sharp;
using Moq;
using Semver;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.Plugin.Git.Tests.UnitTests
{
    public class GitChangelogAdapterTests
    {
        private MockRepository _mockRepository { get; }

        public GitChangelogAdapterTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Run_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = null;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));

            Mock<IRepositoryFactory> mockRepositoryFactory = _mockRepository.Create<IRepositoryFactory>();

            GitChangelogAdapter adapter = new GitChangelogAdapter(mockRepositoryFactory.Object);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => adapter.Run(mockPipelineContext.Object, null));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_AddsGitPluginStepIfChangelogsLastestVersionHasNoCorrespondingTag()
        {
            // Arrange
            string testVersion = "1.0.0";

            Mock<IVersion> mockVersion = _mockRepository.Create<IVersion>();
            mockVersion.Setup(v => v.SemVersion).Returns(SemVersion.Parse(testVersion));
            SortedSet<IVersion> versions = new SortedSet<IVersion> { mockVersion.Object };
            Mock<IChangelog> mockChangelog = _mockRepository.Create<IChangelog>();
            mockChangelog.Setup(c => c.Versions).Returns(versions);

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = mockChangelog.Object;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));

            Mock<IRepository> mockRepository = _mockRepository.Create<IRepository>();
            Mock<TagCollection> mockTags = Mock.Get(mockRepository.Object.Tags);
            mockTags.Setup(t => t[testVersion]).Returns((Tag)null);
            Mock<IRepositoryFactory> mockRepositoryFactory = _mockRepository.Create<IRepositoryFactory>();
            mockRepositoryFactory.Setup(r => r.Build(It.IsAny<string>())).Returns(mockRepository.Object);

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            LinkedList<IStep> steps = new LinkedList<IStep>();
            mockStepContext.Setup(s => s.RemainingSteps).Returns(steps);

            GitChangelogAdapter adapter = new GitChangelogAdapter(mockRepositoryFactory.Object);

            // Act 
            adapter.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            Assert.Equal(1, steps.Count);
            GitPluginOptions options = steps.First.Value.PluginOptions as GitPluginOptions;
            Assert.Equal(testVersion, options.TagName);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_DoesNotAddAnyStepsIfLatestChangelogVersionHasACorrespondingTags()
        {
            // Arrange
            string testVersion = "1.0.0";

            Mock<IVersion> mockVersion = _mockRepository.Create<IVersion>();
            mockVersion.Setup(v => v.SemVersion).Returns(SemVersion.Parse(testVersion));
            SortedSet<IVersion> versions = new SortedSet<IVersion> { mockVersion.Object };
            Mock<IChangelog> mockChangelog = _mockRepository.Create<IChangelog>();
            mockChangelog.Setup(c => c.Versions).Returns(versions);

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = mockChangelog.Object;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));

            Mock<IRepository> mockRepository = _mockRepository.Create<IRepository>();
            Mock<TagCollection> mockTags = Mock.Get(mockRepository.Object.Tags);
            mockTags.Setup(t => t[testVersion]).Returns((Tag)null);
            Mock<IRepositoryFactory> mockRepositoryFactory = _mockRepository.Create<IRepositoryFactory>();
            mockRepositoryFactory.Setup(r => r.Build(It.IsAny<string>())).Returns(mockRepository.Object);

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            LinkedList<IStep> steps = new LinkedList<IStep>();
            mockStepContext.Setup(s => s.RemainingSteps).Returns(steps);

            GitChangelogAdapter adapter = new GitChangelogAdapter(mockRepositoryFactory.Object);

            // Act 
            adapter.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            Assert.Equal(1, steps.Count);
            GitPluginOptions options = steps.First.Value.PluginOptions as GitPluginOptions;
            Assert.Equal(testVersion, options.TagName);
            _mockRepository.VerifyAll();
        }
    }
}
