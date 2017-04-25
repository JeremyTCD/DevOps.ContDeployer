using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GitTags.Tests.UnitTests
{
    [Collection(nameof(GitTagsCollection))]
    public class GitTagsTests
    {
        private Signature _signature { get; }

        public GitTagsTests(GitTagsFixture fixture)
        {
            fixture.ResetTempDir();
            _signature = fixture.Signature;
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateMockStepContext();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitTags(null, stepContext));
        }

        [Theory]
        [MemberData(nameof(RunThrowsExceptionIfTagNameIsNullOrEmptyData))]
        public void Run_ThrowsExceptionIfTagNameIsNullOrEmpty(string testTagName)
        {
            // Arrange
            IStepContext stepContext = PluginTestHelpers.CreateMockStepContext(new GitTagsOptions { TagName = testTagName });

            GitTags gitTags = new GitTags(null, stepContext);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => gitTags.Run());
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
            IPipelineContext pipelineContext = PluginTestHelpers.CreateMockPipelineContext();
            IStepContext stepContext = PluginTestHelpers.CreateMockStepContext(new GitTagsOptions { TagName = testTagName });

            // Test commit for tag to point to
            File.WriteAllText("test.txt", "test");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            GitTags gitTags = new GitTags(pipelineContext, stepContext);

            // Act
            gitTags.Run();

            // Assert
            Assert.NotNull(pipelineContext.Repository.Tags[testTagName]);
        }

        [Fact]
        public void Run_DoesNothingOnDryRun()
        {
            // Arrange
            string testTagName = "0.1.0";
            IPipelineContext pipelineContext = PluginTestHelpers.CreateMockPipelineContext();
            IStepContext stepContext = PluginTestHelpers.CreateMockStepContext(new GitTagsOptions { TagName = testTagName });

            // Test commit for tag to point to
            File.WriteAllText("test.txt", "test");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            GitTags gitTags = new GitTags(pipelineContext, stepContext);

            // Act
            gitTags.Run();

            // Assert
            Assert.Null(pipelineContext.Repository.Tags[testTagName]);
        }

        [Fact]
        public void Run_ThrowsExceptionIfGitTagFails()
        {
            // Arrange
            string testTagName = "0.1.0";
            IPipelineContext pipelineContext = PluginTestHelpers.CreateMockPipelineContext();
            IStepContext stepContext = PluginTestHelpers.CreateMockStepContext(new GitTagsOptions { TagName = testTagName });

            // Test commit for tag to point to
            File.WriteAllText("test.txt", "test");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);

            pipelineContext.Repository.ApplyTag(testTagName);

            GitTags gitTags = new GitTags(pipelineContext, stepContext);

            // Act
            Assert.Throws<Exception>(() => gitTags.Run());
        }
    }
}
