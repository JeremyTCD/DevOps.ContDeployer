using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.Changelog.Tests.UnitTests
{
    public class ChangelogPluginUnitTests
    {
        private MockRepository _mockRepository { get; }

        public ChangelogPluginUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNullOrNotAChangelogPluginOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns((IPluginOptions)null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new ChangelogPlugin(null, mockStepContext.Object, null));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ThrowsExceptionIfRepositoryHasNoCommits()
        {
            // Arrange
            string testCommitish = "HEAD";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns(new ChangelogPluginOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IRepository> mockRepository = Mock.Get(mockPipelineContext.Object.Repository);
            mockRepository.Setup(r => r.Lookup(testCommitish)).Returns((Commit)null);

            ChangelogPlugin changelogPlugin = new ChangelogPlugin(mockPipelineContext.Object, mockStepContext.Object, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogPlugin.Run());
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileDoesNotExist()
        {
            // Arrange
            string testFileName = "testFileName";
            string testCommitish = "HEAD";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns(new ChangelogPluginOptions { FileName = testFileName });

            Mock<Commit> mockCommit = _mockRepository.Create<Commit>();
            mockCommit.Setup(c => c[testFileName]).Returns((TreeEntry)null);
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IRepository> mockRepository = Mock.Get(mockPipelineContext.Object.Repository);
            mockRepository.Setup(r => r.Lookup(testCommitish)).Returns(mockCommit.Object);

            ChangelogPlugin changelogPlugin = new ChangelogPlugin(mockPipelineContext.Object, mockStepContext.Object, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogPlugin.Run());
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileIsEmpty()
        {
            // Arrange
            string testFileName = "testFileName";
            string testChangelogText = "";
            string testCommitish = "HEAD";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(o => o.Options).Returns(new ChangelogPluginOptions { FileName = testFileName });

            Mock<Blob> mockBlob = _mockRepository.Create<Blob>();
            mockBlob.Setup(b => b.GetContentStream()).Returns(CreateStreamFromString(testChangelogText));
            Mock<TreeEntry> mockTreeEntry = _mockRepository.Create<TreeEntry>();
            mockTreeEntry.Setup(t => t.Target).Returns(mockBlob.Object);
            Mock<Commit> mockCommit = _mockRepository.Create<Commit>();
            mockCommit.Setup(c => c[testFileName]).Returns(mockTreeEntry.Object);
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IRepository> mockRepository = Mock.Get(mockPipelineContext.Object.Repository);
            mockRepository.Setup(r => r.Lookup(testCommitish)).Returns(mockCommit.Object);

            ChangelogPlugin changelogPlugin = new ChangelogPlugin(mockPipelineContext.Object, mockStepContext.Object, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogPlugin.Run());
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_GeneratesChangelogAndInsertsItIntoSharedDataIfSuccessful()
        {
            // Arrange
            string testFileName = "testFileName";
            string testChangelogText = "testChangelogText";
            string testPattern = "testPattern";
            string testCommitish = "HEAD";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(o => o.Options).Returns(new ChangelogPluginOptions { FileName = testFileName,
                Pattern = testPattern });

            Mock<IChangelog> mockChangelog = _mockRepository.Create<IChangelog>();
            Mock<IChangelogFactory> mockChangelogFactory = _mockRepository.Create<IChangelogFactory>();
            mockChangelogFactory.Setup(c => c.Build(testPattern, testChangelogText)).Returns(mockChangelog.Object);

            Mock<Blob> mockBlob = _mockRepository.Create<Blob>();
            mockBlob.Setup(b => b.GetContentStream()).Returns(CreateStreamFromString(testChangelogText));
            Mock<TreeEntry> mockTreeEntry = _mockRepository.Create<TreeEntry>();
            mockTreeEntry.Setup(t => t.Target).Returns(mockBlob.Object);
            Mock<Commit> mockCommit = _mockRepository.Create<Commit>();
            mockCommit.Setup(c => c[testFileName]).Returns(mockTreeEntry.Object);
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IRepository> mockRepository = Mock.Get(mockPipelineContext.Object.Repository);
            mockRepository.Setup(r => r.Lookup(testCommitish)).Returns(mockCommit.Object);

            ChangelogPlugin changelogPlugin = new ChangelogPlugin(mockPipelineContext.Object, mockStepContext.Object, 
                mockChangelogFactory.Object);

            // Act
            changelogPlugin.Run();

            // Assert
            _mockRepository.VerifyAll();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            mockSharedData.VerifySet(s => s[nameof(Changelog)] = mockChangelog.Object);
        }

        private Stream CreateStreamFromString(string value)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

    }
}
