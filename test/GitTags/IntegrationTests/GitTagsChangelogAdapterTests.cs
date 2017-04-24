using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using LibGit2Sharp;
using Semver;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GitTags.IntegrationTests
{
    [Collection(nameof(GitTagsCollection))]
    public class GitTagsChangelogAdapterTests
    {
        private Signature _signature { get; }

        public GitTagsChangelogAdapterTests(GitTagsFixture fixture)
        {
            fixture.ResetTempDir();
            _signature = fixture.Signature;
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            PipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitTagsChangelogAdapter(pipelineContext, null));
        }

        [Fact]
        public void Run_AddsGitTagsStepIfChangelogsLastestVersionHasNoCorrespondingTag()
        {
            // Arrange
            PipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();
            string testVersion = "1.0.0";
            SortedSet<IVersion> versions = new SortedSet<IVersion>()
            {
                new ChangelogGenerator.Version { SemVersion = SemVersion.Parse(testVersion) }
            };
            Changelog changelog = new Changelog(versions);
            pipelineContext.SharedData.Add(nameof(Changelog), changelog);

            GitTagsChangelogAdapter gitTagsChangelogAdapter = new GitTagsChangelogAdapter(pipelineContext, 
                PluginTestHelpers.CreateStepContext());

            // Act 
            gitTagsChangelogAdapter.Run();

            // Assert
            Assert.Equal(1, pipelineContext.Steps.Count);
            Assert.Equal(nameof(GitTags), pipelineContext.Steps.First.Value.PluginName);
            Assert.Equal(testVersion, (pipelineContext.Steps.First.Value.Options as GitTagsOptions).TagName);
        }

        [Fact]
        public void Run_DoesNothingIfChangelogVersionsAllHaveCorrespondingTags()
        {
            // Arrange
            PipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();
            string testVersion = "1.0.0";
            // Create tag
            File.WriteAllText("test.txt", "test");
            Commands.Stage(pipelineContext.Repository, "*");
            pipelineContext.Repository.Commit("Initial commit", _signature, _signature);
            pipelineContext.Repository.ApplyTag(testVersion);

            SortedSet<IVersion> versions = new SortedSet<IVersion>()
            {
                new ChangelogGenerator.Version { SemVersion = SemVersion.Parse(testVersion) }
            };
            Changelog changelog = new Changelog(versions);
            pipelineContext.SharedData.Add(nameof(Changelog), changelog);

            GitTagsChangelogAdapter gitTagsChangelogAdapter = new GitTagsChangelogAdapter(pipelineContext,
                PluginTestHelpers.CreateStepContext());

            // Act 
            gitTagsChangelogAdapter.Run();

            // Assert
            Assert.Equal(0, pipelineContext.Steps.Count);
        }
    }
}
