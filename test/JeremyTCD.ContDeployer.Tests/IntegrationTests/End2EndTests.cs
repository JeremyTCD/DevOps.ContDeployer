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
                Pipeline = new PipelineOptions{
                    PipelineSteps = new List<PipelineStep> {
                                new PipelineStep
                                {
                                    PluginName = "ChangelogDeployer"
                                }
                            }
                }
            };
            string optionsJson = JsonConvert.SerializeObject(options, _serializerSettings);
            File.WriteAllText("cd.json", optionsJson);

            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Initial commit", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.2.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 2", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.3.0\nBody");
            Commands.Stage(_repository, "*");
            _repository.Commit("Commit 3", _signature, _signature);

            // Act
            ContDeployer.Main(null);

            // Assert
        }
}
}
