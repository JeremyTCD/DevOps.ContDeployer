using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Semver;
using System;
using System.Collections.Generic;
using System.IO;
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
        public void Run_AddsTagGeneratorStepIfChangelogsLastestVersionHasNoCorrespondingTag()
        {
            // Arrange
            string testVersion = "1.0.0";
            SortedSet<ChangelogGenerator.Version> versions = new SortedSet<ChangelogGenerator.Version>()
            {
                new ChangelogGenerator.Version { SemVersion = SemVersion.Parse(testVersion) }
            };
            Changelog changelog = new Changelog(versions);
            _pipelineContext.SharedData.Add(nameof(Changelog), changelog);

            TagGeneratorChangelogAdapter tagGeneratorChangelogAdapter = new TagGeneratorChangelogAdapter();

            // Act 
            tagGeneratorChangelogAdapter.Run(_pipelineContext, _stepContext);

            // Assert
            Assert.Equal(1, _pipelineContext.Steps.Count);
            Assert.Equal(nameof(TagGenerator), _pipelineContext.Steps.First.Value.PluginName);
            Assert.Equal(testVersion, (_pipelineContext.Steps.First.Value.Options as TagGeneratorOptions).TagName);
        }

        [Fact]
        public void Run_DoesNothingIfChangelogVersionsAllHaveCorrespondingTags()
        {
            // Arrange
            File.WriteAllText("test.txt", "test");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            string testVersion = "1.0.0";
            SortedSet<ChangelogGenerator.Version> versions = new SortedSet<ChangelogGenerator.Version>()
            {
                new ChangelogGenerator.Version { SemVersion = SemVersion.Parse(testVersion) }
            };
            Changelog changelog = new Changelog(versions);
            _pipelineContext.SharedData.Add(nameof(Changelog), changelog);

            _pipelineContext.Repository.ApplyTag(testVersion);

            TagGeneratorChangelogAdapter tagGeneratorChangelogAdapter = new TagGeneratorChangelogAdapter();

            // Act 
            tagGeneratorChangelogAdapter.Run(_pipelineContext, _stepContext);

            // Assert
            Assert.Equal(0, _pipelineContext.Steps.Count);
        }
    }
}
