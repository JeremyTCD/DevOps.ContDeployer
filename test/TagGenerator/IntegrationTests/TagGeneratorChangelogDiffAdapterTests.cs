using JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Semver;
using System;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator.IntegrationTests
{
    [Collection(nameof(TagGeneratorCollection))]
    public class TagGeneratorChangelogDiffAdapterTests
    {
        private JsonSerializerSettings _serializerSettings { get; }
        private Signature _signature { get; }
        private PipelineContext _pipelineContext { get; }
        private StepContext _stepContext { get; }

        public TagGeneratorChangelogDiffAdapterTests(TagGeneratorFixture fixture)
        {
            fixture.ResetTempDir();
            _serializerSettings = fixture.SerializerSettings;
            _signature = fixture.Signature;
            _pipelineContext = fixture.CreatePipelineContext();
            _stepContext = fixture.
                CreateStepContext((new Mock<ILogger>()).Object, // TODO Create a dummy logger
                    new ChangelogDiffGeneratorOptions());
        }

        [Fact]
        public void Run_ThrowsExceptionIfSharedDataDoesNotContainChangelogDiff()
        {
            // Arrange
            Mock<ILogger<TagGeneratorChangelogDiffAdapter>> mockLogger = new Mock<ILogger<TagGeneratorChangelogDiffAdapter>>();

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGeneratorChangelogDiffAdapter.Run(_pipelineContext, null));
        }

        [Fact]
        public void Run_AddsTagGeneratorStepIfChangelogDiffContainsAnAddedVersion()
        {
            // Arrange
            Mock<ILogger<TagGeneratorChangelogDiffAdapter>> mockLogger = new Mock<ILogger<TagGeneratorChangelogDiffAdapter>>();

            string testVersion = "1.0.0";
            ChangelogDiff diff = new ChangelogDiff();
            diff.AddedVersions.Add(new ChangelogDiffGenerator.Version { SemVersion = SemVersion.Parse(testVersion) });
            _pipelineContext.SharedData.Add(nameof(ChangelogDiff), diff);

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act 
            tagGeneratorChangelogDiffAdapter.Run(_pipelineContext, _stepContext);

            // Assert
            Assert.Equal(1, _pipelineContext.Steps.Count);
            Assert.Equal(nameof(TagGenerator), _pipelineContext.Steps.First.Value.PluginName);
            Assert.Equal(testVersion, (_pipelineContext.Steps.First.Value.Options as TagGeneratorOptions).TagName);
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogDiffContainsMoreThanOneAddedVersions()
        {
            // Arrange
            ChangelogDiff diff = new ChangelogDiff();
            diff.AddedVersions.Add(new ChangelogDiffGenerator.Version());
            diff.AddedVersions.Add(new ChangelogDiffGenerator.Version());
            _pipelineContext.SharedData.Add(nameof(ChangelogDiff), diff);

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGeneratorChangelogDiffAdapter.Run(_pipelineContext, _stepContext));
        }

        [Fact]
        public void Run_DoesNothingIfChangelogDiffDoesNotContainAnyAddedVersions()
        {
            // Arrange
            ChangelogDiff diff = new ChangelogDiff();
            _pipelineContext.SharedData.Add(nameof(ChangelogDiff), diff);

            TagGeneratorChangelogDiffAdapter tagGeneratorChangelogDiffAdapter = new TagGeneratorChangelogDiffAdapter();

            // Act 
            tagGeneratorChangelogDiffAdapter.Run(_pipelineContext, _stepContext);

            // Assert
            Assert.Equal(0, _pipelineContext.Steps.Count);
        }
    }
}
