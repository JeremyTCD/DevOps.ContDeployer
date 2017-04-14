using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator.IntegrationTests
{
    [Collection(nameof(ChangelogDiffGeneratorCollection))]
    public class ChangelogDiffGeneratorTests
    {
        private string _tempDir { get; }
        private string _tempPluginsDir { get; }
        private JsonSerializerSettings _serializerSettings { get; }
        private Repository _repository { get; }
        private Signature _signature { get; }

        public ChangelogDiffGeneratorTests(ChangelogDiffGeneratorFixture fixture)
        {
            fixture.ResetTempDir();
            _tempDir = fixture.TempDir;
            _tempPluginsDir = fixture.TempPluginsDir;
            _serializerSettings = fixture.SerializerSettings;
            _repository = fixture.Repository;
            _signature = fixture.Signature;
        }

        [Fact]
        public void Run_ThrowsExceptionIfRepositoryHasNoCommits()
        {
            // Arrange
            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();

            PipelineContext pipelineContext = CreatePipelineContext();
            PipelineStepContext pipelineStepContext = CreatePipelineStepContext();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.
                Run(pipelineContext, pipelineStepContext));
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileDoesNotExist()
        {
            // Arrange
            File.WriteAllText("test.txt", "test");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            PipelineContext pipelineContext = CreatePipelineContext();
            PipelineStepContext pipelineStepContext = CreatePipelineStepContext();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.
                Run(pipelineContext, pipelineStepContext));
        }

        [Fact]
        public void Run_DoesNothingIfChangelogHasNotChanged()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            File.WriteAllText("test.txt", "test");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 2", _signature, _signature);

            PipelineContext pipelineContext = CreatePipelineContext();
            PipelineStepContext pipelineStepContext = CreatePipelineStepContext();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator();

            // Act
            changelogDiffGenerator.Run(pipelineContext, pipelineStepContext);

            // Assert
            Assert.Equal(0, pipelineContext.PipelineSteps.Count);
        }

        [Fact]
        public void Run_GeneratesDiffIfFirstCommitAndChangelogHasBeenAddedToIndex()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            PipelineContext pipelineContext = CreatePipelineContext();
            PipelineStepContext pipelineStepContext = CreatePipelineStepContext();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator();

            // Act 
            changelogDiffGenerator.Run(pipelineContext, pipelineStepContext);

            // Assert
            pipelineContext.SharedData.TryGetValue(nameof(ChangelogDiff), out object diff);
            Assert.NotNull(diff);
        }

        [Fact]
        public void Run_GeneratesDiffIfChangelogAddedToIndexInLastCommit()
        {
            // Arrange
            File.WriteAllText("test.txt", "test");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 2", _signature, _signature);

            PipelineContext pipelineContext = CreatePipelineContext();
            PipelineStepContext pipelineStepContext = CreatePipelineStepContext();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator();

            // Act 
            changelogDiffGenerator.Run(pipelineContext, pipelineStepContext);

            // Assert
            pipelineContext.SharedData.TryGetValue(nameof(ChangelogDiff), out object diff);
            Assert.NotNull(diff);
        }

        [Fact]
        public void Run_ThrowsExceptionIfMoreThanOneVersionAdded()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.2.0\nBody\n## 0.3.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 2", _signature, _signature);

            PipelineContext pipelineContext = CreatePipelineContext();
            PipelineStepContext pipelineStepContext = CreatePipelineStepContext();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.Run(pipelineContext, pipelineStepContext));
        }

        [Fact]
        public void Run_ThrowsExceptionIfVersionHasBeenRemoved()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.2.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 2", _signature, _signature);

            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 3", _signature, _signature);

            PipelineContext pipelineContext = CreatePipelineContext();
            PipelineStepContext pipelineStepContext = CreatePipelineStepContext();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.Run(pipelineContext, pipelineStepContext));
        }

        [Fact]
        public void Run_GeneratesDiffIfVersionAddedToChangelogInLastCommit()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            File.AppendAllText("changelog.md", "## 0.2.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 2", _signature, _signature);

            PipelineContext pipelineContext = CreatePipelineContext();
            PipelineStepContext pipelineStepContext = CreatePipelineStepContext();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator();

            // Act 
            changelogDiffGenerator.Run(pipelineContext, pipelineStepContext);

            // Assert
            pipelineContext.SharedData.TryGetValue(nameof(ChangelogDiff), out object diff);
            Assert.NotNull(diff);
        }

        private PipelineContext CreatePipelineContext()
        {
            Dictionary<string, object> sharedData = new Dictionary<string, object>();
            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>();

            return new PipelineContext {
                Repository = _repository,
                SharedData = sharedData,
                PipelineSteps = steps
            };
        }

        private PipelineStepContext CreatePipelineStepContext()
        {
            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();
            ChangelogDiffGeneratorOptions options = new ChangelogDiffGeneratorOptions();

            return new PipelineStepContext
            {
                Logger = mockLogger.Object,
                Options = options
            };
        }
    }
}
