using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator.IntegrationTests
{
    [Collection(nameof(TagGeneratorCollection))]
    public class TagGeneratorTests
    {
        private JsonSerializerSettings _serializerSettings { get; }
        private Signature _signature { get; }
        private TagGeneratorFixture _fixture { get; }

        public TagGeneratorTests(TagGeneratorFixture fixture)
        {
            fixture.ResetTempDir();
            _fixture = fixture;
            _serializerSettings = fixture.SerializerSettings;
            _signature = fixture.Signature;
        }

        [Fact]
        public void Run_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            PipelineContext pipelineContext = _fixture.CreatePipelineContext();
            StepContext stepContext = _fixture.
                CreateStepContext((new Mock<ILogger>()).Object,
                    new TagGeneratorOptions());

            stepContext.Options = null;

            TagGenerator tagGenerator = new TagGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGenerator.
                Run(pipelineContext, stepContext));
        }

        [Theory]
        [MemberData(nameof(RunThrowsExceptionIfTagNameIsNullOrEmptyData))]
        public void Run_ThrowsExceptionIfTagNameIsNullOrEmpty(string testTagName)
        {
            // Arrange
            PipelineContext pipelineContext = _fixture.CreatePipelineContext();
            StepContext stepContext = _fixture.
                CreateStepContext((new Mock<ILogger>()).Object,
                    new TagGeneratorOptions());

            ((TagGeneratorOptions)stepContext.Options).TagName = testTagName;

            TagGenerator tagGenerator = new TagGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGenerator.
                Run(pipelineContext, stepContext));
        }

        public static IEnumerable<object[]> RunThrowsExceptionIfTagNameIsNullOrEmptyData()
        {
            yield return new object[] { null };
            yield return new object[] { "" };
        }

        [Fact]
        public void Run_CreatesTagWithSpecifiedName()
        {
            // Arrange
            PipelineContext pipelineContext = _fixture.CreatePipelineContext();
            StepContext stepContext = _fixture.
                CreateStepContext((new Mock<ILogger>()).Object,
                    new TagGeneratorOptions());

            string testTagName = "0.1.0";
            ((TagGeneratorOptions)stepContext.Options).TagName = testTagName;

            // Test commit for tag to point to
            File.WriteAllText("test.txt", "test");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            TagGenerator tagGenerator = new TagGenerator();

            // Act
            tagGenerator.Run(pipelineContext, stepContext);

            // Assert
            Assert.NotNull(pipelineContext.Repository.Tags[testTagName]);
        }

        [Fact]
        public void Run_DoesNothingOnDryRun()
        {
            // Arrange
            PipelineContext pipelineContext = _fixture.CreatePipelineContext(new SharedOptions { DryRun = true });
            StepContext stepContext = _fixture.
                CreateStepContext((new Mock<ILogger>()).Object,
                    new TagGeneratorOptions());

            string testTagName = "0.1.0";
            ((TagGeneratorOptions)stepContext.Options).TagName = testTagName;

            // Test commit for tag to point to
            File.WriteAllText("test.txt", "test");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            TagGenerator tagGenerator = new TagGenerator();

            // Act
            tagGenerator.Run(pipelineContext, stepContext);

            // Assert
            Assert.Null(pipelineContext.Repository.Tags[testTagName]);
        }

        [Fact]
        public void Run_ThrowsExceptionIfGitTagFails()
        {
            // Arrange
            PipelineContext pipelineContext = _fixture.CreatePipelineContext();
            StepContext stepContext = _fixture.
                CreateStepContext((new Mock<ILogger>()).Object,
                    new TagGeneratorOptions());

            string testTagName = "0.1.0";
            ((TagGeneratorOptions)stepContext.Options).TagName = testTagName;

            // Test commit for tag to point to
            File.WriteAllText("test.txt", "test");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            pipelineContext.Repository.ApplyTag(testTagName);

            TagGenerator tagGenerator = new TagGenerator();

            // Act
            Assert.Throws<Exception>(() => tagGenerator.Run(pipelineContext, stepContext));
        }
    }
}
