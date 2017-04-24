using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using LibGit2Sharp;
using NSubstitute;
using System;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.Tests.UnitTests
{
    public class ChangelogGeneratorTests
    {
        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            IStepContext mockStepContext = PluginTestHelpers.CreateStepContext(null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new ChangelogGenerator(null, mockStepContext, null));
        }

        [Fact]
        public void Run_ThrowsExceptionIfRepositoryHasNoCommits()
        {
            // Arrange
            IStepContext mockStepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions());
            IRepository mockRepository = PluginTestHelpers.CreateRepository(); // IRepository.Lookup<Commit>() returns null by default
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreatePipelineContext(repository: mockRepository);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext, mockStepContext, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
            mockRepository.Received().Lookup<Commit>("HEAD");
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileDoesNotExist()
        {
            // Arrange
            string testFileName = "testFileName";
            string testCommitish = "HEAD";

            IStepContext mockStepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions
            {
                FileName = testFileName
            });
            Commit mockCommit = PluginTestHelpers.CreateCommit(); // Commit[string] returns null by default
            IRepository mockRepository = PluginTestHelpers.CreateRepository(testCommitish, mockCommit);
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreatePipelineContext(repository: mockRepository);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext, mockStepContext, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
            var _ = mockCommit.Received()[testFileName];
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileIsEmpty()
        {
            // Arrange
            string testFileName = "testFileName";
            string testCommitish = "HEAD";

            IStepContext mockStepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions
            {
                FileName = testFileName
            });
            Blob mockBlob = PluginTestHelpers.CreateBlob("");
            TreeEntry mockTreeEntry = PluginTestHelpers.CreateTreeEntry(mockBlob);
            Commit mockCommit = PluginTestHelpers.CreateCommit(testFileName, mockTreeEntry);
            IRepository mockRepository = PluginTestHelpers.CreateRepository(testCommitish, mockCommit);
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreatePipelineContext(repository: mockRepository);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext, mockStepContext, null);

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

            IStepContext mockStepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions
            {
                FileName = testFileName,
                Pattern = testPattern
            });
            Blob mockBlob = PluginTestHelpers.CreateBlob(testChangelogText);
            TreeEntry mockTreeEntry = PluginTestHelpers.CreateTreeEntry(mockBlob);
            Commit mockCommit = PluginTestHelpers.CreateCommit(testFileName, mockTreeEntry);
            IRepository mockRepository = PluginTestHelpers.CreateRepository(testCommitish, mockCommit);
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreatePipelineContext(repository: mockRepository);
            IChangelog mockChangelog = ChangelogGeneratorTestHelpers.CreateChangelog();
            IChangelogFactory mockChangelogFactory = ChangelogGeneratorTestHelpers.CreateChangelogFactory(mockChangelog, testPattern, testChangelogText);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext, mockStepContext, mockChangelogFactory);

            // Act
            changelogGenerator.Run();

            // Assert
            mockPipelineContext.SharedData.Received()[nameof(Changelog)] = mockChangelog;
        }
    }
}
