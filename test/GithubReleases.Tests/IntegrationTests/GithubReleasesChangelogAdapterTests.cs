using JeremyTCD.ContDeployer.Plugin.GithubReleases;
using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleases.IntegrationTests
{
    [Collection(nameof(GithubReleasesCollection))]
    public class GithubReleasesChangelogAdapterTests
    {
        private JsonSerializerSettings _serializerSettings { get; }
        private Signature _signature { get; }
        private GithubReleasesFixture _fixture { get; }

        public GithubReleasesChangelogAdapterTests(GithubReleasesFixture fixture)
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
            StepContext stepContext = _fixture.CreateStepContext(null, null);

            GithubReleasesChangelogAdapter adapter = new GithubReleasesChangelogAdapter();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => adapter.Run(null, stepContext));
        }

        [Fact]
        public void Run_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            PipelineContext pipelineContext = _fixture.CreatePipelineContext();
            StepContext stepContext = _fixture.
                            CreateStepContext(null, new GithubReleasesOptions());

            GithubReleasesChangelogAdapter adapter = new GithubReleasesChangelogAdapter();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => adapter.Run(null, stepContext));
        }
    }
}
