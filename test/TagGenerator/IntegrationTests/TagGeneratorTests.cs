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
        private PipelineContext _pipelineContext { get; }
        private StepContext _stepContext { get; }

        public TagGeneratorTests(TagGeneratorFixture fixture)
        {
            fixture.ResetTempDir();
            _serializerSettings = fixture.SerializerSettings;
            _signature = fixture.Signature;
            _pipelineContext = fixture.CreatePipelineContext();
            _stepContext = fixture.
                CreateStepContext((new Mock<ILogger>()).Object, // TODO Create a dummy logger
                    new TagGeneratorOptions());
        }

        [Fact]
        public void Run_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            _stepContext.Options = null;

            TagGenerator tagGenerator = new TagGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGenerator.
                Run(_pipelineContext, _stepContext));
        }

        [Theory]
        [MemberData(nameof(RunThrowsExceptionIfTagNameIsNullOrEmptyData))]
        public void Run_ThrowsExceptionIfTagNameIsNullOrEmpty(string testTagName)
        {
            // Arrange
            ((TagGeneratorOptions)_stepContext.Options).TagName = testTagName;

            TagGenerator tagGenerator = new TagGenerator();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => tagGenerator.
                Run(_pipelineContext, _stepContext));
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
            string testTagName = "0.1.0";
            ((TagGeneratorOptions)_stepContext.Options).TagName = testTagName;

            // Test commit for tag to point to
            File.WriteAllText("test.txt", "test");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            TagGenerator tagGenerator = new TagGenerator();

            // Act
            tagGenerator.Run(_pipelineContext, _stepContext);

            // Assert
            Assert.NotNull(_pipelineContext.Repository.Tags[testTagName]);
        }

        [Fact]
        public void Run_ThrowsExceptionIfGitTagFails()
        {
            // Arrange
            string testTagName = "0.1.0";
            ((TagGeneratorOptions)_stepContext.Options).TagName = testTagName;

            // Test commit for tag to point to
            File.WriteAllText("test.txt", "test");
            Commands.Stage(_pipelineContext.Repository, "*");
            _pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            _pipelineContext.Repository.ApplyTag(testTagName);

            TagGenerator tagGenerator = new TagGenerator();

            // Act
            Assert.Throws<Exception>(() => tagGenerator.Run(_pipelineContext, _stepContext));
        }
    }
}
