using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.Tests;
using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using LibGit2Sharp;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GitTags.Tests.UnitTests
{
    public class GitTagsChangelogAdapterTests
    {
        [Fact]
        public void Constructor_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            IDictionary<string, object> sharedData = PluginTestHelpers.CreateSharedData();
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreateMockPipelineContext(sharedData);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GitTagsChangelogAdapter(mockPipelineContext, null));
        }

        [Fact]
        public void Run_AddsGitTagsStepIfChangelogsLastestVersionHasNoCorrespondingTag()
        {
            // Arrange
            string testVersion = "1.0.0";

            SortedSet<IVersion> versions = ChangelogGeneratorTestHelpers.CreateVersions(testVersion);
            IChangelog mockChangelog = ChangelogGeneratorTestHelpers.CreateMockChangelog(versions);
            IDictionary<string, object> sharedData = PluginTestHelpers.CreateSharedData(nameof(Changelog), mockChangelog);
            TagCollection mockTags = PluginTestHelpers.CreateMockTags();
            IRepository mockRepository = PluginTestHelpers.CreateMockRepository(tags: mockTags);
            IStep mockStep = PluginTestHelpers.CreateMockStep(nameof(GitTags));
            IStepFactory mockStepFactory = PluginTestHelpers.CreateMockStepFactory(nameof(GitTags), mockStep);
            LinkedList<IStep> steps = PluginTestHelpers.CreateSteps();
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreateMockPipelineContext(sharedData, mockRepository,
                mockStepFactory, steps);

            IStepContext stepContext = PluginTestHelpers.CreateMockStepContext();

            GitTagsChangelogAdapter gitTagsChangelogAdapter = new GitTagsChangelogAdapter(mockPipelineContext, stepContext);

            // Act 
            gitTagsChangelogAdapter.Run();

            // Assert
            var _ = mockTags.Received()[testVersion];
            Assert.Equal(1, mockPipelineContext.Steps.Count);
            GitTagsOptions generatedOptions = mockStepFactory.ReceivedCalls().First().GetArguments()[1] as GitTagsOptions;
            Assert.NotNull(generatedOptions);
            Assert.Equal(testVersion, generatedOptions.TagName);
        }

        [Fact]
        public void Run_DoesNothingIfChangelogVersionsAllHaveCorrespondingTags()
        {
            // Arrange
            string testVersion = "1.0.0";

            SortedSet<IVersion> versions = ChangelogGeneratorTestHelpers.CreateVersions(testVersion);
            IChangelog mockChangelog = ChangelogGeneratorTestHelpers.CreateMockChangelog(versions);
            IDictionary<string, object> sharedData = PluginTestHelpers.CreateSharedData(nameof(Changelog), mockChangelog);
            Tag tag = PluginTestHelpers.CreateMockTag();
            TagCollection mockTags = PluginTestHelpers.CreateMockTags(testVersion, tag);
            IRepository mockRepository = PluginTestHelpers.CreateMockRepository(tags: mockTags);
            IStep mockStep = PluginTestHelpers.CreateMockStep(nameof(GitTags));
            IStepFactory mockStepFactory = PluginTestHelpers.CreateMockStepFactory(nameof(GitTags), mockStep);
            LinkedList<IStep> steps = PluginTestHelpers.CreateSteps();
            IPipelineContext mockPipelineContext = PluginTestHelpers.CreateMockPipelineContext(sharedData, mockRepository,
                mockStepFactory, steps);

            IStepContext stepContext = PluginTestHelpers.CreateMockStepContext();

            GitTagsChangelogAdapter gitTagsChangelogAdapter = new GitTagsChangelogAdapter(mockPipelineContext, stepContext);

            // Act 
            gitTagsChangelogAdapter.Run();

            // Assert
            Assert.Equal(0, mockPipelineContext.Steps.Count);
        }
    }
}
