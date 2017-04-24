using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.Tests;
using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using LibGit2Sharp;
using NSubstitute;
using System;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.UnitTests
{
    public class ChangelogGeneratorTests
    {
        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateStepContext((ChangelogGeneratorOptions)null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new ChangelogGenerator(null, stepContext, null));
        }

        [Fact]
        public void Run_ThrowsExceptionIfRepositoryHasNoCommits()
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions());
            IRepository repository = PluginTestHelpers.CreateRepository(); // IRepository.Lookup<Commit>() returns null by default
            IPipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext(repository);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(pipelineContext, stepContext, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
            repository.Received().Lookup<Commit>("HEAD");
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileDoesNotExist()
        {
            // Arrange
            string testFileName = "testFileName";
            string testCommitish = "HEAD";

            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions
            {
                FileName = testFileName
            });
            Commit commit = PluginTestHelpers.CreateCommit(); // Commit[string] returns null by default
            IRepository repository = PluginTestHelpers.CreateRepository(testCommitish, commit);
            IPipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext(repository);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(pipelineContext, stepContext, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
            var _ = commit.Received()[testFileName];
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileIsEmpty()
        {
            // Arrange
            string testFileName = "testFileName";
            string testCommitish = "HEAD";

            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions
            {
                FileName = testFileName
            });
            Blob blob = PluginTestHelpers.CreateBlob("");
            TreeEntry treeEntry = PluginTestHelpers.CreateTreeEntry(blob);
            Commit commit = PluginTestHelpers.CreateCommit(testFileName, treeEntry);
            IRepository repository = PluginTestHelpers.CreateRepository(testCommitish, commit);
            IPipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext(repository);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(pipelineContext, stepContext, null);

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

            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions
            {
                FileName = testFileName,
                Pattern = testPattern
            });
            Blob blob = PluginTestHelpers.CreateBlob(testChangelogText);
            TreeEntry treeEntry = PluginTestHelpers.CreateTreeEntry(blob);
            Commit commit = PluginTestHelpers.CreateCommit(testFileName, treeEntry);
            IRepository repository = PluginTestHelpers.CreateRepository(testCommitish, commit);
            IPipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext(repository);
            IChangelog changelog = ChangelogGeneratorTestHelpers.CreateChangelog();
            IChangelogFactory changelogFactory = ChangelogGeneratorTestHelpers.CreateChangelogFactory(changelog, testPattern, testChangelogText);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(pipelineContext, stepContext, changelogFactory);

            // Act
            changelogGenerator.Run();

            // Assert
            pipelineContext.SharedData.Received()[nameof(Changelog)] = changelog;
        }
    }
}
