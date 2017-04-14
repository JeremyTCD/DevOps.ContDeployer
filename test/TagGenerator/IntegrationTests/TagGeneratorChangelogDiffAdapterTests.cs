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

            StepContext stepsContext = CreateStepContext();

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act 
            tagGeneratorChangelogDiffAdapter.Run(pipelineContext, stepsContext);

            // Assert
            Assert.Equal(1, pipelineContext.Steps.Count);
            Assert.Equal(nameof(TagGenerator), pipelineContext.Steps.First.Value.PluginName);
            Assert.Equal(testVersion, (pipelineContext.Steps.First.Value.Options as TagGeneratorOptions).TagName);
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

            StepContext stepsContext = CreateStepContext();

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGeneratorChangelogDiffAdapter.Run(pipelineContext, stepsContext));
        }

        [Fact]
        public void Run_DoesNothingIfChangelogDiffDoesNotContainAnyAddedVersions()
        {
            // Arrange
            ChangelogDiff diff = new ChangelogDiff();

            PipelineContext pipelineContext = CreatePipelineContext();
            pipelineContext.SharedData.Add(nameof(ChangelogDiff), diff);

            StepContext stepsContext = CreateStepContext();

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act 
            tagGeneratorChangelogDiffAdapter.Run(pipelineContext, stepsContext);

            // Assert
            Assert.Equal(0, pipelineContext.Steps.Count);
        }

        private PipelineContext CreatePipelineContext()
        {
            Dictionary<string, object> sharedData = new Dictionary<string, object>();
            LinkedList<Step> steps = new LinkedList<Step>();

            return new PipelineContext
            {
                Repository = _repository,
                SharedData = sharedData,
                Steps = steps
            };
        }

        private StepContext CreateStepContext()
        {
            Mock<ILogger<TagGeneratorChangelogDiffAdapter>> mockLogger = new Mock<ILogger<TagGeneratorChangelogDiffAdapter>>();

            return new StepContext
            {
                Logger = mockLogger.Object
            };
        }
    }
}
