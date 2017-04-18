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
        public void Run_DoesNothingIfChangelogHasNotChanged()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            File.WriteAllText("test.txt", "test");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Commit 2", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator();

            // Act
            changelogGenerator.Run(_pipelineContext, _stepContext);

            // Assert
            Assert.Equal(0, _pipelineContext.Steps.Count);
        }

        [Fact]
        public void Run_GeneratesDiffIfFirstCommitAndChangelogHasBeenAddedToIndex()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator();

            // Act 
            changelogGenerator.Run(_pipelineContext, _stepContext);

            // Assert
            _pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object diff);
            Assert.NotNull(diff);
        }

        [Fact]
        public void Run_GeneratesDiffIfChangelogAddedToIndexInLastCommit()
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

        [Fact]
        public void Run_ThrowsExceptionIfMoreThanOneVersionAdded()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.2.0\nBody\n## 0.3.0\nBody");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Commit 2", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run(_pipelineContext, _stepContext));
        }

        [Fact]
        public void Run_ThrowsExceptionIfVersionHasBeenRemoved()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.2.0\nBody");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Commit 2", _signature, _signature);

            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Commit 3", _signature, _signature);

            ChangelogGenerator changelogGenerator = new ChangelogGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogGenerator.Run(_pipelineContext, _stepContext));
        }

        [Fact]
        public void Run_GeneratesDiffIfVersionAddedToChangelogInLastCommit()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            File.AppendAllText("changelog.md", "## 0.2.0\nBody");
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
