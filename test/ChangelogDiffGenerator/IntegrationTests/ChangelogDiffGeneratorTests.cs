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

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(), 
                mockLogger.Object, 
                _repository);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.Run(null, null));
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogFileDoesNotExist()
        {
            // Arrange
            File.WriteAllText("test.txt", "test");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();
            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(), 
                mockLogger.Object,
                _repository);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.Run(null, null));
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

            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();
            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(), 
                mockLogger.Object, 
                _repository);
            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>();

            Dictionary<string, object> sharedData = new Dictionary<string, object>();

            // Act
            changelogDiffGenerator.Run(sharedData, null);

            // Assert
            Assert.Equal(0, sharedData.Count);
        }

        [Fact]
        public void Run_GeneratesDiffIfFirstCommitAndChangelogHasBeenAddedToIndex()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);
                   
            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();
        
            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(),
                mockLogger.Object,
                _repository);

            Dictionary<string, object> sharedData = new Dictionary<string, object>();

            // Act 
            changelogDiffGenerator.Run(sharedData, null);

            // Assert
            sharedData.TryGetValue(nameof(ChangelogDiff), out object diff);
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

            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(),
                mockLogger.Object,
                _repository);

            Dictionary<string, object> sharedData = new Dictionary<string, object>();

            // Act 
            changelogDiffGenerator.Run(sharedData, null);

            // Assert
            sharedData.TryGetValue(nameof(ChangelogDiff), out object diff);
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

            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(),
                mockLogger.Object,
                _repository);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.Run(null, null));
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

            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(),
                mockLogger.Object,
                _repository);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.Run(null, null));
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

            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();

            ChangelogDiffGenerator changelogDiffGenerator = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(),
                mockLogger.Object,
                _repository);

            Dictionary<string, object> sharedData = new Dictionary<string, object>();

            // Act 
            changelogDiffGenerator.Run(sharedData, null);

            // Assert
            sharedData.TryGetValue(nameof(ChangelogDiff), out object diff);
            Assert.NotNull(diff);
        }
    }
}
