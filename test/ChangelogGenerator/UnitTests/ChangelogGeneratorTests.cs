using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.Tests.UnitTests
{
    public class ChangelogGeneratorTests
    {
        private MockRepository _mockRepository { get; }

        public ChangelogGeneratorTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNullOrNotAChangelogGeneratorOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns((IPluginOptions)null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new ChangelogGenerator(null, mockStepContext.Object, null));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ThrowsExceptionIfRepositoryHasNoCommits()
        {
            // Arrange
            string testCommitish = "HEAD";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns(new ChangelogGeneratorOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IRepository> mockRepository = Mock.Get(mockPipelineContext.Object.Repository);
            mockRepository.Setup(r => r.Lookup(testCommitish)).Returns((Commit)null);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext.Object, mockStepContext.Object, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileDoesNotExist()
        {
            // Arrange
            string testFileName = "testFileName";
            string testCommitish = "HEAD";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns(new ChangelogGeneratorOptions { FileName = testFileName });

            Mock<Commit> mockCommit = _mockRepository.Create<Commit>();
            mockCommit.Setup(c => c[testFileName]).Returns((TreeEntry)null);
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IRepository> mockRepository = Mock.Get(mockPipelineContext.Object.Repository);
            mockRepository.Setup(r => r.Lookup(testCommitish)).Returns(mockCommit.Object);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext.Object, mockStepContext.Object, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
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
            mockStepContext.Setup(o => o.Options).Returns(new ChangelogGeneratorOptions { FileName = testFileName });

            Mock<Blob> mockBlob = _mockRepository.Create<Blob>();
            mockBlob.Setup(b => b.GetContentStream()).Returns(CreateStreamFromString(testChangelogText));
            Mock<TreeEntry> mockTreeEntry = _mockRepository.Create<TreeEntry>();
            mockTreeEntry.Setup(t => t.Target).Returns(mockBlob.Object);
            Mock<Commit> mockCommit = _mockRepository.Create<Commit>();
            mockCommit.Setup(c => c[testFileName]).Returns(mockTreeEntry.Object);
            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IRepository> mockRepository = Mock.Get(mockPipelineContext.Object.Repository);
            mockRepository.Setup(r => r.Lookup(testCommitish)).Returns(mockCommit.Object);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext.Object, mockStepContext.Object, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
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
            mockStepContext.Setup(o => o.Options).Returns(new ChangelogGeneratorOptions { FileName = testFileName, Pattern = testPattern });

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

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext.Object, mockStepContext.Object, 
                mockChangelogFactory.Object);

            // Act
            changelogGenerator.Run();

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
