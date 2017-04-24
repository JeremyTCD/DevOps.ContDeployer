using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.IntegrationTests
{
    [Collection(nameof(ChangelogGeneratorCollection))]
    public class ChangelogGeneratorTests
    {
        private Signature _signature { get; }

        public ChangelogGeneratorTests(ChangelogGeneratorFixture fixture)
        {
            fixture.ResetTempDir();
            _signature = fixture.Signature;
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateStepContext();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new ChangelogGenerator(null, stepContext));
        }

        [Fact]
        public void Run_ThrowsExceptionIfRepositoryHasNoCommits()
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions());
            IPipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(pipelineContext, stepContext);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileDoesNotExist()
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions());
            IPipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();

            File.WriteAllText("test.txt", "test");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(pipelineContext, stepContext);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileIsEmpty()
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions());
            IPipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();

            File.WriteAllText("changelog.md", "");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(pipelineContext, stepContext);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run());
        }

        [Fact]
        public void Run_GeneratesChangelogAndInsertsItIntoSharedDataIfSuccessful()
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateStepContext(new ChangelogGeneratorOptions());
            IPipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();

            File.WriteAllText("test.txt", "test");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Commit 2", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator(pipelineContext, stepContext);

            // Act 
            changelogGenerator.Run();

            // Assert
            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object diff);
            Assert.NotNull(diff);
        }
    }
}
