using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
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
    public class TagGeneratorChangelogAdapterTests
    {
        private JsonSerializerSettings _serializerSettings { get; }
        private Signature _signature { get; }
        private PipelineContext _pipelineContext { get; }
        private StepContext _stepContext { get; }

        public TagGeneratorChangelogAdapterTests(TagGeneratorFixture fixture)
        {
            fixture.ResetTempDir();
            _serializerSettings = fixture.SerializerSettings;
            _signature = fixture.Signature;
            _pipelineContext = fixture.CreatePipelineContext();
            _stepContext = fixture.
                CreateStepContext((new Mock<ILogger>()).Object, // TODO Create a dummy logger
                    null);
        }

        [Fact]
        public void Run_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            Mock<ILogger<TagGeneratorChangelogAdapter>> mockLogger = new Mock<ILogger<TagGeneratorChangelogAdapter>>();

            TagGeneratorChangelogAdapter tagGeneratorChangelogAdapter = new TagGeneratorChangelogAdapter();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGeneratorChangelogAdapter.Run(_pipelineContext, null));
        }

        [Fact]
        public void Run_AddsTagGeneratorStepIfChangelogContainsAnAddedVersion()
        {
            // Arrange
            Mock<ILogger<TagGeneratorChangelogAdapter>> mockLogger = new Mock<ILogger<TagGeneratorChangelogAdapter>>();

            string testVersion = "1.0.0";
            Changelog diff = new Changelog();
            diff.AddedVersions.Add(new ChangelogGenerator.Version { SemVersion = SemVersion.Parse(testVersion) });
            _pipelineContext.SharedData.Add(nameof(Changelog), diff);

            TagGeneratorChangelogAdapter tagGeneratorChangelogAdapter = new TagGeneratorChangelogAdapter();

            // Act 
            tagGeneratorChangelogAdapter.Run(_pipelineContext, _stepContext);

            // Assert
            Assert.Equal(1, _pipelineContext.Steps.Count);
            Assert.Equal(nameof(TagGenerator), _pipelineContext.Steps.First.Value.PluginName);
            Assert.Equal(testVersion, (_pipelineContext.Steps.First.Value.Options as TagGeneratorOptions).TagName);
        }

        [Fact]
        public void Run_ThrowsExceptionIfChangelogContainsMoreThanOneAddedVersions()
        {
            // Arrange
            Changelog diff = new Changelog();
            diff.AddedVersions.Add(new ChangelogGenerator.Version());
            diff.AddedVersions.Add(new ChangelogGenerator.Version());
            _pipelineContext.SharedData.Add(nameof(Changelog), diff);

            TagGeneratorChangelogAdapter tagGeneratorChangelogAdapter = new TagGeneratorChangelogAdapter();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGeneratorChangelogAdapter.Run(_pipelineContext, _stepContext));
        }

        [Fact]
        public void Run_DoesNothingIfChangelogDoesNotContainAnyAddedVersions()
        {
            // Arrange
            Changelog diff = new Changelog();
            _pipelineContext.SharedData.Add(nameof(Changelog), diff);

            TagGeneratorChangelogAdapter tagGeneratorChangelogAdapter = new TagGeneratorChangelogAdapter();

            // Act 
            tagGeneratorChangelogAdapter.Run(_pipelineContext, _stepContext);

            // Assert
            Assert.Equal(0, _pipelineContext.Steps.Count);
        }
    }
}
