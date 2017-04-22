using JeremyTCD.ContDeployer.Plugin.GithubReleases;
using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Tests.IntegrationTests
{
    [Collection(nameof(ContDeployerCollection))]
    public class ContDeployerTests
    {
        private string _tempDir { get; }
        private JsonSerializerSettings _serializerSettings { get; }
        private Signature _signature { get; }

        public ContDeployerTests(ContDeployerFixture fixture)
        {
            fixture.ResetTempDir();
            _tempDir = fixture.TempDir;
            _serializerSettings = fixture.SerializerSettings;
            _signature = fixture.Signature;
        }

        [Fact]
        public void DefaultPipline_GeneratesTagGithubReleaseAndPublishesIfNewVersionHasBeenAdded()
        {
            // Arrange
            Repository repository = new Repository(_tempDir);

            // TODO run in dry run mode so no side effects
            object options = new
            {
                Pipeline = new
                {
                    Steps = new List<object> {
                                new
                                {
                                    PluginName = "ChangelogGenerator"
                                },
                                new
                                {
                                    PluginName = "GitTagsChangelogAdapter"
                                },
                                new
                                {
                                    PluginName = "GithubReleasesChangelogAdapter",
                                    Config = new
                                    {
                                        Owner = "testOwner",
                                        Repository = "testRepository",
                                        Token = "testToken"
                                    }
                                }
                            }
                }
            };
            string optionsJson = JsonConvert.SerializeObject(options, _serializerSettings);
            File.WriteAllText("cd.json", optionsJson);

            // Create test commits
            File.WriteAllText("changelog.md", "## 0.1.0\nBody");
            Commands.Stage(repository, "*");
            repository.Commit("Initial commit", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.2.0\nBody");
            Commands.Stage(repository, "*");
            repository.Commit("Commit 2", _signature, _signature);

            File.AppendAllText("changelog.md", "\n## 0.3.0\nBody");
            Commands.Stage(repository, "*");
            repository.Commit("Commit 3", _signature, _signature);

            IConfigurationRoot configurationRoot = GetConfigurationRoot();
            Container main = GetInstanceCollection(configurationRoot);
            // TODO override githubclient services with mock
            Configure(configurationRoot, main);

            Pipeline pipeline = main.GetInstance<Pipeline>();

            // Act
            pipeline.Run();

            // Assert
            repository = new Repository(_tempDir);
            Assert.NotNull(repository.Tags["0.3.0"]);
        }

        private Container GetInstanceCollection(IConfigurationRoot configurationRoot)
        {
            Container container = new Container();
            container.AddContDeployer(configurationRoot);

            return container;
        }

        private IConfigurationRoot GetConfigurationRoot()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("cd.json");

            return builder.Build();
        }

        private void Configure(IConfigurationRoot configurationRoot, Container main)
        {
            ILoggerFactory loggerFactory = main.GetInstance<ILoggerFactory>();
            IPluginFactory pluginFactory = main.GetInstance<IPluginFactory>();
            StepContextFactory stepContextFactory = main.GetInstance<StepContextFactory>();

            loggerFactory.
                AddFile(configurationRoot.GetValue("Logging:File:LogFile", "log.txt"),
                    (configurationRoot.GetValue("Logging:File:LogLevel", Microsoft.Extensions.Logging.LogLevel.Information))).
                AddConsole(configurationRoot.GetValue("Logging:Console:LogLevel", Microsoft.Extensions.Logging.LogLevel.Information)).
                AddDebug(configurationRoot.GetValue("Logging:Debug:LogLevel", Microsoft.Extensions.Logging.LogLevel.Information));
            pluginFactory.LoadTypes();
            stepContextFactory.LoadTypes();
        }
    }
}
