using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using NSubstitute;
using System;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.Tests.UnitTests
{
    public class ChangelogGeneratorTests
    {
        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNullOrNotAChangelogGeneratorOptionsInstance()
        {
            // Arrange
            IStepContext mockStepContext = Substitute.For<IStepContext>();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new ChangelogGenerator(null, mockStepContext, null));
        }

        [Fact]
        public void Run_ThrowsExceptionIfRepositoryHasNoCommits()
        {
            // Arrange
            string testCommitish = "HEAD";

            ChangelogGeneratorOptions mockOptions = Substitute.For<ChangelogGeneratorOptions>();
            IStepContext mockStepContext = Substitute.For<IStepContext>();
            mockStepContext.Options.Returns(mockOptions);

            IPipelineContext mockPipelineContext = Substitute.For<IPipelineContext>();
            mockPipelineContext.Repository.Lookup<Commit>(testCommitish).Returns((Commit)null);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext, mockStepContext, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileDoesNotExist()
        {
            // Arrange
            string testFileName = "testFileName";
            string testCommitish = "HEAD";

            ChangelogGeneratorOptions mockOptions = Substitute.For<ChangelogGeneratorOptions>();
            mockOptions.FileName.Returns(testFileName);
            IStepContext mockStepContext = Substitute.For<IStepContext>();
            mockStepContext.Options.Returns(mockOptions);

            Commit mockCommit = Substitute.For<Commit>();
            mockCommit[testFileName].Returns((TreeEntry)null);
            IPipelineContext mockPipelineContext = Substitute.For<IPipelineContext>();
            mockPipelineContext.Repository.Lookup<Commit>(testCommitish).Returns(mockCommit);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext, mockStepContext, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileIsEmpty()
        {
            // Arrange
            string testFileName = "testFileName";
            string testChangelogText = "";
            string testCommitish = "HEAD";

            ChangelogGeneratorOptions mockOptions = Substitute.For<ChangelogGeneratorOptions>();
            mockOptions.FileName.Returns(testFileName);
            IStepContext mockStepContext = Substitute.For<IStepContext>();
            mockStepContext.Options.Returns(mockOptions);

            Blob mockBlob = Substitute.For<Blob>();
            mockBlob.GetContentStream().Returns(CreateStreamFromString(testChangelogText));
            TreeEntry mockTreeEntry = Substitute.For<TreeEntry>();
            mockTreeEntry.Target.Returns(mockBlob);
            Commit mockCommit = Substitute.For<Commit>();
            mockCommit[testFileName].Returns(mockTreeEntry);
            IPipelineContext mockPipelineContext = Substitute.For<IPipelineContext>();
            mockPipelineContext.Repository.Lookup<Commit>(testCommitish).Returns(mockCommit);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext, mockStepContext,
                null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
        }

        [Fact]
        public void Run_GeneratesChangelogAndInsertsItIntoSharedDataIfSuccessful()
        {
            // Arrange
            string testFileName = "testFileName";
            string testChangelogText = "testChangelogText";
            string testPattern = "testPattern";
            string testCommitish = "HEAD";

            ChangelogGeneratorOptions mockOptions = Substitute.For<ChangelogGeneratorOptions>();
            mockOptions.FileName.Returns(testFileName);
            mockOptions.Pattern.Returns(testPattern);
            IStepContext mockStepContext = Substitute.For<IStepContext>();
            mockStepContext.Options.Returns(mockOptions);

            Blob mockBlob = Substitute.For<Blob>();
            mockBlob.GetContentStream().Returns(CreateStreamFromString(testChangelogText));
            TreeEntry mockTreeEntry = Substitute.For<TreeEntry>();
            mockTreeEntry.Target.Returns(mockBlob);
            Commit mockCommit = Substitute.For<Commit>();
            mockCommit[testFileName].Returns(mockTreeEntry);
            IPipelineContext mockPipelineContext = Substitute.For<IPipelineContext>();
            mockPipelineContext.Repository.Lookup<Commit>(testCommitish).Returns(mockCommit);

            IChangelog mockChangelog = Substitute.For<IChangelog>();
            IChangelogFactory mockChangelogFactory = Substitute.For<IChangelogFactory>();
            mockChangelogFactory.Build(testPattern, testChangelogText).Returns(mockChangelog);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext, mockStepContext, 
                mockChangelogFactory);

            // Act
            changelogGenerator.Run();

            // Assert
            Assert.Equal(mockChangelog, mockPipelineContext.SharedData[nameof(Changelog)]);
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
