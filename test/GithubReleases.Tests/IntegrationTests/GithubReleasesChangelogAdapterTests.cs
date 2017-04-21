using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.ContDeployer.PluginTools.Tests;
using LibGit2Sharp;
using Newtonsoft.Json;
using System;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleases.IntegrationTests
{
    [Collection(nameof(GithubReleasesCollection))]
    public class GithubReleasesChangelogAdapterTests
    {
        private Signature _signature { get; }

        public GithubReleasesChangelogAdapterTests(GithubReleasesFixture fixture)
        {
            fixture.ResetTempDir();
            _signature = fixture.Signature;
        }

        [Fact]
        public void Run_ThrowsExceptionIfOptionsIsNull()
        {
            // Arrange
            StepContext stepContext = PluginTestHelpers.CreateStepContext();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GithubReleasesChangelogAdapter(null, stepContext));
        }

        [Fact]
        public void Run_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            PipelineContext pipelineContext = PluginTestHelpers.CreatePipelineContext();
            StepContext stepContext = PluginTestHelpers.CreateStepContext(new GithubReleasesChangelogAdapterOptions());

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new GithubReleasesChangelogAdapter(pipelineContext, stepContext));
        }
    }
}
