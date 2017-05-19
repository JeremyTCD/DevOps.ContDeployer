using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Moq;
using System;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.Git.Tests.UnitTests
{
    public class GitPluginUnitTests
    {
        private MockRepository _mockRepository { get; }

        public GitPluginUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNotAGitOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns((IPluginOptions)null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitPlugin(null, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_CreatesTagWithSpecifiedName()
        {
            // Arrange
            string testTagName = "0.1.0";
            string testName = "testName";
            string testEmail = "testEmail";
            string testCommitish = "testCommitish";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns(new GitPluginOptions
            {
                Commitish = testCommitish,
                Email = testEmail,
                Name = testName,
                TagName = testTagName
            });

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<SharedOptions> mockSharedOptions = Mock.Get(mockPipelineContext.Object.SharedOptions);
            mockSharedOptions.Setup(s => s.DryRun).Returns(false);

            Mock<IRepository> mockRepository = Mock.Get(mockPipelineContext.Object.Repository);
            Mock<GitObject> mockGitObject = _mockRepository.Create<GitObject>();
            mockRepository.Setup(r => r.Lookup(testCommitish)).Returns(mockGitObject.Object);
            Mock<TagCollection> mockTags = Mock.Get(mockPipelineContext.Object.Repository.Tags);
            mockTags.
                Setup(t => t.Add(testTagName, mockGitObject.Object, 
                    It.Is<Signature>(s => s.Name == testName && s.Email == testEmail), ""));

            GitPlugin gitPlugin = new GitPlugin(mockPipelineContext.Object, mockStepContext.Object);

            // Act
            gitPlugin.Run();

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_DoesNotAddATagOnDryRun()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns(new GitPluginOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<SharedOptions> mockSharedOptions = Mock.Get(mockPipelineContext.Object.SharedOptions);
            mockSharedOptions.Setup(s => s.DryRun).Returns(true);
            
            GitPlugin gitPlugin = new GitPlugin(mockPipelineContext.Object, mockStepContext.Object);

            // Act
            gitPlugin.Run();

            // Assert
            _mockRepository.VerifyAll();
            Mock<TagCollection> mockTags = Mock.Get(mockPipelineContext.Object.Repository.Tags);
            mockTags.
                Verify(t => t.Add(It.IsAny<string>(), It.IsAny<GitObject>(), It.IsAny<Signature>(), It.IsAny<string>()), Times.Never);
        }
    }
}
