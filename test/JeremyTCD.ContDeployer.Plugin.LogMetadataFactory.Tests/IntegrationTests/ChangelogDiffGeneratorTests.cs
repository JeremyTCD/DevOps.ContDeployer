using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using JeremyTCD.ContDeployer.PluginTools;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Serialization;
using LibGit2Sharp;
using Moq;
using Microsoft.Extensions.Logging;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator.IntegrationTests
{
    [Collection(nameof(LogMetadataFactoryCollection))]
    public class ChangelogDiffGeneratorTests
    {
        private string _tempDir { get; }
        private string _tempPluginsDir { get; }
        private JsonSerializerSettings _serializerSettings { get; }
        private Repository _repository { get; }
        private Signature _signature { get; }

        public ChangelogDiffGeneratorTests(LogMetadataFactoryFixture fixture)
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

            ChangelogDiffGenerator logMetadataFactory = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(), 
                mockLogger.Object, 
                _repository);

            // Act and Assert
            Assert.Throws<Exception>(() => logMetadataFactory.Run(null));
        }

        [Fact]
        public void Run_DoesNothingIfChangelogFileDoesNotExist()
        {
            // Arrange
            File.WriteAllText("test.txt", "test");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();
            ChangelogDiffGenerator logMetadataFactory = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(), 
                mockLogger.Object,
                _repository);
            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>();

            // Act
            logMetadataFactory.Run(steps);

            // Assert
            Assert.Equal(0, steps.Count);
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
            ChangelogDiffGenerator logMetadataFactory = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(), 
                mockLogger.Object, 
                _repository);
            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>();

            // Act
            logMetadataFactory.Run(steps);

            // Assert
            Assert.Equal(0, steps.Count);
        }

        [Fact]
        public void Run_GeneratesPipelineIfFirstCommitAndChangelogHasBeenAdded()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);
            
            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();
        
            ChangelogDiffGenerator logMetadataFactory = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(),
                mockLogger.Object,
                _repository);

            // Act and Assert
        }

        [Fact]
        public void Run_GeneratesPipelineIfVersionHasBeenAdded()
        {
            // Arrange
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.2.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 2", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.3.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 3", _signature, _signature);

            Mock<ILogger<ChangelogDiffGenerator>> mockLogger = new Mock<ILogger<ChangelogDiffGenerator>>();

            ChangelogDiffGenerator logMetadataFactory = new ChangelogDiffGenerator(new ChangelogDiffGeneratorOptions(),
                mockLogger.Object,
                _repository);
            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>();

            // Act
            logMetadataFactory.Run(steps);

            // Assert
        }
    }
}
