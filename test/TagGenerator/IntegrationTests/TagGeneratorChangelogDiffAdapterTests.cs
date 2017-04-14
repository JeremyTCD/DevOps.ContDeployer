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
    public class TagGeneratorChangelogDiffAdapterTests
    {
        private string _tempDir { get; }
        private string _tempPluginsDir { get; }
        private JsonSerializerSettings _serializerSettings { get; }
        private Repository _repository { get; }
        private Signature _signature { get; }

        public TagGeneratorChangelogDiffAdapterTests(TagGeneratorFixture fixture)
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
            Mock<ILogger<TagGeneratorChangelogDiffAdapter>> mockLogger = new Mock<ILogger<TagGeneratorChangelogDiffAdapter>>();

            PipelineContext pipelineContext = CreatePipelineContext();

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGeneratorChangelogDiffAdapter.Run(pipelineContext, null));
        }

        [Fact]
        public void Run_AddsTagGeneratorStepIfChangelogDiffContainsAnAddedVersion()
        {
            // Arrange
            Mock<ILogger<TagGeneratorChangelogDiffAdapter>> mockLogger = new Mock<ILogger<TagGeneratorChangelogDiffAdapter>>();

            string testVersion = "1.0.0";
            ChangelogDiff diff = new ChangelogDiff();
            diff.AddedVersions.Add(new ChangelogDiffGenerator.Version { SemVersion = SemVersion.Parse(testVersion) });

            PipelineContext pipelineContext = CreatePipelineContext();
            pipelineContext.SharedData.Add(nameof(ChangelogDiff), diff);

            PipelineStepContext pipelineStepsContext = CreatePipelineStepContext();

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act 
            tagGeneratorChangelogDiffAdapter.Run(pipelineContext, pipelineStepsContext);

            // Assert
            Assert.Equal(1, pipelineContext.PipelineSteps.Count);
            Assert.Equal(nameof(TagGenerator), pipelineContext.PipelineSteps.First.Value.PluginName);
            Assert.Equal(testVersion, (pipelineContext.PipelineSteps.First.Value.Options as TagGeneratorOptions).TagName);
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogDiffContainsMoreThanOneAddedVersions()
        {
            // Arrange
            ChangelogDiff diff = new ChangelogDiff();
            diff.AddedVersions.Add(new ChangelogDiffGenerator.Version());
            diff.AddedVersions.Add(new ChangelogDiffGenerator.Version());

            PipelineContext pipelineContext = CreatePipelineContext();
            pipelineContext.SharedData.Add(nameof(ChangelogDiff), diff);

            PipelineStepContext pipelineStepsContext = CreatePipelineStepContext();

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGeneratorChangelogDiffAdapter.Run(pipelineContext, pipelineStepsContext));
        }

        [Fact]
        public void Run_DoesNothingIfChangelogDiffDoesNotContainAnyAddedVersions()
        {
            // Arrange
            ChangelogDiff diff = new ChangelogDiff();

            PipelineContext pipelineContext = CreatePipelineContext();
            pipelineContext.SharedData.Add(nameof(ChangelogDiff), diff);

            PipelineStepContext pipelineStepsContext = CreatePipelineStepContext();

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act 
            tagGeneratorChangelogDiffAdapter.Run(pipelineContext, pipelineStepsContext);

            // Assert
            Assert.Equal(0, pipelineContext.PipelineSteps.Count);
        }

        private PipelineContext CreatePipelineContext()
        {
            Dictionary<string, object> sharedData = new Dictionary<string, object>();

            return new PipelineContext
            {
                Repository = _repository,
                SharedData = sharedData
            };
        }

        private PipelineStepContext CreatePipelineStepContext()
        {
            Mock<ILogger<TagGeneratorChangelogDiffAdapter>> mockLogger = new Mock<ILogger<TagGeneratorChangelogDiffAdapter>>();

            return new PipelineStepContext
            {
                Logger = mockLogger.Object
            };
        }
    }
}
