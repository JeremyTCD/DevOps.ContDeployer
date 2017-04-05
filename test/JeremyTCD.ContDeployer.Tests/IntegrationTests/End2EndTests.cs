using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using JeremyTCD.ContDeployer.PluginTools;
using Newtonsoft.Json;
using System.IO;

namespace JeremyTCD.ContDeployer.Tests.IntegrationTests
{
    [Collection(nameof(E2ECollection))]
    public class End2EndTests
    {
        private string _tempDir { get; }
        private string _tempPluginsDir { get; }

        public End2EndTests(E2EFixture fixture)
        {
            _tempDir = fixture.TempDir;
            _tempPluginsDir = fixture.TempPluginsDir;
            fixture.ResetTempDir();
        }

        [Fact]
        public void DefaultPipline_GeneratesTagGithubReleaseAndPublishesIfNewVersionHasBeenAdded()
        {
            // Arrange
            object options = new
            {
                Pipeline = new
                {
                    PipelineSteps = new PipelineStep[] {
                                new PipelineStep {PluginName="LogMetadataFactory"}
                            }
                }
            };
            string optionsJson = JsonConvert.SerializeObject(options);
            File.WriteAllText("cd.json", optionsJson);

            // Act
            ContDeployer.Main(null);

            // Assert
        }
    }
}
