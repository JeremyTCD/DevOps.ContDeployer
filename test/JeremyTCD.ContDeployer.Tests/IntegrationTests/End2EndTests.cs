using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using JeremyTCD.ContDeployer.PluginTools;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Serialization;
using LibGit2Sharp;

namespace JeremyTCD.ContDeployer.Tests.IntegrationTests
{
    [Collection(nameof(E2ECollection))]
    public class End2EndTests
    {
        private string _tempDir { get; }
        private string _tempPluginsDir { get; }
        private JsonSerializerSettings _serializerSettings { get; }
        private Repository _repository { get; }
        private Signature _signature { get; }

        public End2EndTests(E2EFixture fixture)
        {
            fixture.ResetTempDir();
            _tempDir = fixture.TempDir;
            _tempPluginsDir = fixture.TempPluginsDir;
            _serializerSettings = fixture.SerializerSettings;
            _repository = fixture.Repository;
            _signature = fixture.Signature;
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
            string optionsJson = JsonConvert.SerializeObject(options, _serializerSettings);
            File.WriteAllText("cd.json", optionsJson);

            // Act
            ContDeployer.Main(null);

            // Assert
        }
    }
}
