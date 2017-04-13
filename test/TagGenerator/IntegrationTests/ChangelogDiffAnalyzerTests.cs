using JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Semver;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator.IntegrationTests
{
    [Collection(nameof(TagGeneratorCollection))]
    public class ChangelogDiffAnalyzerTests
    {
        private string _tempDir { get; }
        private string _tempPluginsDir { get; }
        private JsonSerializerSettings _serializerSettings { get; }
        private Repository _repository { get; }
        private Signature _signature { get; }

        public ChangelogDiffAnalyzerTests(TagGeneratorFixture fixture)
        {
            fixture.ResetTempDir();
            _tempDir = fixture.TempDir;
            _tempPluginsDir = fixture.TempPluginsDir;
            _serializerSettings = fixture.SerializerSettings;
            _repository = fixture.Repository;
            _signature = fixture.Signature;
        }

        [Fact]
        public void Run_ThrowsExceptionIfSharedDataDoesNotContainChangelogDiff()
        {
            // Arrange
            Mock<ILogger<ChangelogDiffAnalyzer>> mockLogger = new Mock<ILogger<ChangelogDiffAnalyzer>>();

            ChangelogDiffAnalyzer changelogDiffGenerator = new ChangelogDiffAnalyzer(mockLogger.Object,
                _repository);

            Dictionary<string, object> sharedData = new Dictionary<string, object>();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.Run(sharedData, null));
        }

        [Fact]
        public void Run_AddsTagGeneratorStepIfChangelogDiffContainsAnAddedVersion()
        {
            // Arrange
            Mock<ILogger<ChangelogDiffAnalyzer>> mockLogger = new Mock<ILogger<ChangelogDiffAnalyzer>>();

            ChangelogDiffAnalyzer changelogDiffGenerator = new ChangelogDiffAnalyzer(mockLogger.Object,
                _repository);

            string testVersion = "1.0.0";
            ChangelogDiff diff = new ChangelogDiff();
            diff.AddedVersions.Add(new ChangelogDiffGenerator.Version { SemVersion = SemVersion.Parse(testVersion) });

            Dictionary<string, object> sharedData = new Dictionary<string, object>();
            sharedData.Add(nameof(ChangelogDiff), diff);

            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>();

            // Act 
            changelogDiffGenerator.Run(sharedData, steps);

            // Assert
            Assert.Equal(1, steps.Count);
            Assert.Equal(nameof(TagGenerator), steps.First.Value.PluginName);
            Assert.Equal(testVersion, (steps.First.Value.Options as TagGeneratorOptions).TagName);
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogDiffContainsMoreThanOneAddedVersions()
        {
            // Arrange
            Mock<ILogger<ChangelogDiffAnalyzer>> mockLogger = new Mock<ILogger<ChangelogDiffAnalyzer>>();

            ChangelogDiffAnalyzer changelogDiffGenerator = new ChangelogDiffAnalyzer(mockLogger.Object,
                _repository);

            ChangelogDiff diff = new ChangelogDiff();
            diff.AddedVersions.Add(new ChangelogDiffGenerator.Version());
            diff.AddedVersions.Add(new ChangelogDiffGenerator.Version());

            Dictionary<string, object> sharedData = new Dictionary<string, object>();
            sharedData.Add(nameof(ChangelogDiff), diff);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => changelogDiffGenerator.Run(sharedData, null));
        }

        [Fact]
        public void Run_DoesNothingIfChangelogDiffDoesNotContainAnyAddedVersions()
        {
            // Arrange
            Mock<ILogger<ChangelogDiffAnalyzer>> mockLogger = new Mock<ILogger<ChangelogDiffAnalyzer>>();

            ChangelogDiffAnalyzer changelogDiffGenerator = new ChangelogDiffAnalyzer(mockLogger.Object,
                _repository);

            ChangelogDiff diff = new ChangelogDiff();

            Dictionary<string, object> sharedData = new Dictionary<string, object>();
            sharedData.Add(nameof(ChangelogDiff), diff);

            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>();

            // Act 
            changelogDiffGenerator.Run(sharedData, steps);

            // Assert
            Assert.Equal(0, steps.Count);
        }
    }
}
