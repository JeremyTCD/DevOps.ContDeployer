using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Moq;
using Semver;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GitTags.Tests.UnitTests
{
    public class GitTagsChangelogAdapterTests
    {
        private MockRepository _mockRepository { get; }

        public GitTagsChangelogAdapterTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = null;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitTagsChangelogAdapter(mockPipelineContext.Object, null));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_AddsGitTagsStepIfChangelogsLastestVersionHasNoCorrespondingTag()
        {
            // Arrange
            string testVersion = "1.0.0";

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
            Mock<TagCollection> mockTags = Mock.Get(mockPipelineContext.Object.Repository.Tags);
            mockTags.Setup(t => t[testVersion]).Returns((Tag)null);
            Mock<IStep> mockStep = _mockRepository.Create<IStep>();
            Mock<IStepFactory> mockStepFactory = Mock.Get(mockPipelineContext.Object.StepFactory);
            mockStepFactory.
                Setup(s => s.Build(nameof(GitTags),
                    It.Is<GitTagsOptions>(o => o.TagName == testVersion))).
                Returns(mockStep.Object);

            LinkedList<IStep> steps = new LinkedList<IStep>();
            mockPipelineContext.Setup(o => o.Steps).Returns(steps);

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();

            GitTagsChangelogAdapter gitTagsChangelogAdapter = new GitTagsChangelogAdapter(mockPipelineContext.Object, 
                mockStepContext.Object);

            // Act 
            gitTagsChangelogAdapter.Run();

            // Assert
            Assert.Equal(1, steps.Count);
            Assert.Equal(mockStep.Object, steps.First.Value);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_DoesNotAddAnyStepsIfLatestChangelogVersionHasACorrespondingTags()
        {
            // Arrange
            string testVersion = "1.0.0";

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
            Mock<TagCollection> mockTags = Mock.Get(mockPipelineContext.Object.Repository.Tags);
            mockTags.Setup(t => t[testVersion]).Returns(_mockRepository.Create<Tag>().Object);

            LinkedList<IStep> steps = new LinkedList<IStep>();
            mockPipelineContext.Object.Steps = steps;

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();

            GitTagsChangelogAdapter gitTagsChangelogAdapter = new GitTagsChangelogAdapter(mockPipelineContext.Object,
                mockStepContext.Object);

            // Act 
            gitTagsChangelogAdapter.Run();

            // Assert
            Assert.Equal(0, steps.Count);
            _mockRepository.VerifyAll();
        }
    }
}
