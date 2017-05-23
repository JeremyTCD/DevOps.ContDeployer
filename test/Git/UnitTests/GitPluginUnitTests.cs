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
        public void Run_ThrowsExceptionIfPluginOptionsIsNotAGitPluginOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns((IPluginOptions)null);

            Mock<IRepositoryFactory> mockRepositoryFactory = _mockRepository.Create<IRepositoryFactory>();

            GitPlugin plugin = new GitPlugin(mockRepositoryFactory.Object); 

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => plugin.Run(null, mockStepContext.Object));
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
            mockStepContext.Setup(s => s.PluginOptions).Returns(new GitPluginOptions
            {
                Commitish = testCommitish,
                Email = testEmail,
                Name = testName,
                TagName = testTagName
            });

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<SharedOptions> mockSharedOptions = Mock.Get(mockPipelineContext.Object.SharedOptions);
            mockSharedOptions.Setup(s => s.DryRun).Returns(false);

            Mock<IRepository> mockRepository = _mockRepository.Create<IRepository>();
            Mock<GitObject> mockGitObject = _mockRepository.Create<GitObject>();
            mockRepository.Setup(r => r.Lookup(testCommitish)).Returns(mockGitObject.Object);
            Mock<TagCollection> mockTags = Mock.Get(mockRepository.Object.Tags);
            mockTags.
                Setup(t => t.Add(testTagName, mockGitObject.Object,
                    It.Is<Signature>(s => s.Name == testName && s.Email == testEmail), ""));

            Mock<IRepositoryFactory> mockRepositoryFactory = _mockRepository.Create<IRepositoryFactory>();
            mockRepositoryFactory.Setup(r => r.Build(It.IsAny<string>())).Returns(mockRepository.Object);

            GitPlugin plugin = new GitPlugin(mockRepositoryFactory.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_DoesNotAddATagOnDryRun()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns(new GitPluginOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<SharedOptions> mockSharedOptions = Mock.Get(mockPipelineContext.Object.SharedOptions);
            mockSharedOptions.Setup(s => s.DryRun).Returns(true);

            Mock<IRepository> mockRepository = _mockRepository.Create<IRepository>();
            Mock<IRepositoryFactory> mockRepositoryFactory = _mockRepository.Create<IRepositoryFactory>();
            mockRepositoryFactory.Setup(r => r.Build(It.IsAny<string>())).Returns(mockRepository.Object);

            GitPlugin plugin = new GitPlugin(mockRepositoryFactory.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            Mock<TagCollection> mockTags = Mock.Get(mockRepository.Object.Tags);
            mockTags.
                Verify(t => t.Add(It.IsAny<string>(), It.IsAny<GitObject>(), It.IsAny<Signature>(), It.IsAny<string>()), Times.Never);
        }
    }
}
