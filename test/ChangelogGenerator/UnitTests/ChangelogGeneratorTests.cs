using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using LibGit2Sharp;
using NSubstitute;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.Tests.UnitTests
{
    public class ChangelogGeneratorTests
    {
        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            IStepContext mockStepContext = PluginTestHelpers.CreateMockStepContext(null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new ChangelogGenerator(null, mockStepContext, null));
        }

        [Fact]
        public void Run_ThrowsExceptionIfRepositoryHasNoCommits()
        {
            // Arrange
            IStepContext mockStepContext = PluginTestHelpers.CreateMockStepContext(new ChangelogGeneratorOptions());
            IRepository mockRepository = PluginTestHelpers.CreateMockRepository(); // IRepository.Lookup<Commit>() returns null by default
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreateMockPipelineContext(repository: mockRepository);

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

            IStepContext mockStepContext = PluginTestHelpers.CreateMockStepContext(new ChangelogGeneratorOptions
            {
                FileName = testFileName
            });
            Commit mockCommit = PluginTestHelpers.CreateMockCommit(); // Commit[string] returns null by default
            IRepository mockRepository = PluginTestHelpers.CreateMockRepository(testCommitish, mockCommit);
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreateMockPipelineContext(repository: mockRepository);

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

            IStepContext mockStepContext = PluginTestHelpers.CreateMockStepContext(new ChangelogGeneratorOptions
            {
                FileName = testFileName
            });
            Blob mockBlob = PluginTestHelpers.CreateMockBlob("");
            TreeEntry mockTreeEntry = PluginTestHelpers.CreateMockTreeEntry(mockBlob);
            Commit mockCommit = PluginTestHelpers.CreateMockCommit(testFileName, mockTreeEntry);
            IRepository mockRepository = PluginTestHelpers.CreateMockRepository(testCommitish, mockCommit);
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreateMockPipelineContext(repository: mockRepository);

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

            IStepContext mockStepContext = PluginTestHelpers.CreateMockStepContext(new ChangelogGeneratorOptions
            {
                FileName = testFileName,
                Pattern = testPattern
            });

            Blob mockBlob = PluginTestHelpers.CreateMockBlob(testChangelogText);
            TreeEntry mockTreeEntry = PluginTestHelpers.CreateMockTreeEntry(mockBlob);
            Commit mockCommit = PluginTestHelpers.CreateMockCommit(testFileName, mockTreeEntry);
            IRepository mockRepository = PluginTestHelpers.CreateMockRepository(testCommitish, mockCommit);
            IDictionary<string, object> sharedData = PluginTestHelpers.CreateSharedData();
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreateMockPipelineContext(sharedData, mockRepository);

            IChangelog mockChangelog = ChangelogGeneratorTestHelpers.CreateMockChangelog();
            IChangelogFactory mockChangelogFactory = ChangelogGeneratorTestHelpers.CreateMockChangelogFactory(mockChangelog, 
                testPattern, testChangelogText);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(mockPipelineContext, mockStepContext, 
                mockChangelogFactory);

            // Act
            changelogGenerator.Run();

            // Assert
            sharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            Assert.NotNull(changelogObject as IChangelog);
        }
    }
}
