using JeremyTCD.ContDeployer.PluginTools;
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
        private JsonSerializerSettings _serializerSettings { get; }
        private Signature _signature { get; }
        private PipelineContext _pipelineContext { get; }
        private StepContext _stepContext { get; }

        public ChangelogGeneratorTests(ChangelogGeneratorFixture fixture)
        {
            fixture.ResetTempDir();
            _serializerSettings = fixture.SerializerSettings;
            _signature = fixture.Signature;
            _pipelineContext = fixture.CreatePipelineContext();
            _stepContext = fixture.
                CreateStepContext((new Mock<ILogger>()).Object, // TODO Create a dummy logger
                    new ChangelogGeneratorOptions());
        }

        [Fact]
        public void Run_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            _stepContext.Options = null;

            ChangelogGenerator changelogGenerator = new ChangelogGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.
                Run(_pipelineContext, _stepContext));
        }

        [Fact]
        public void Run_ThrowsExceptionIfRepositoryHasNoCommits()
        {
            // Arrange
            ChangelogGenerator changelogGenerator = new ChangelogGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.
                Run(_pipelineContext, _stepContext));
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileDoesNotExist()
        {
            // Arrange
            File.WriteAllText("test.txt", "test");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.
                Run(_pipelineContext, _stepContext));
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileIsEmpty()
        {
            // Arrange
            File.WriteAllText("changelog.md", "");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.
                Run(_pipelineContext, _stepContext));
        }

        [Fact]
        public void Run_GeneratesChangelogAndInsertsItIntoSharedDataIfSuccessful()
        {
            // Arrange
            File.WriteAllText("test.txt", "test");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Commit 2", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator();

            // Act 
            changelogGenerator.Run(_pipelineContext, _stepContext);

            // Assert
            _pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object diff);
            Assert.NotNull(diff);
        }
    }
}
