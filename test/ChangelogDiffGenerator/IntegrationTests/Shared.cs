using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator.IntegrationTests
{
    [CollectionDefinition(nameof(ChangelogDiffGeneratorCollection))]
    public class ChangelogDiffGeneratorCollection : ICollectionFixture<ChangelogDiffGeneratorFixture>
    {
    }

    public class ChangelogDiffGeneratorFixture : IDisposable
    {
        public string TempDir { get; }
        public string TempPluginsDir { get; }
        public string TempGitDir { get; }
        public JsonSerializerSettings SerializerSettings { get; }
        public Repository Repository { get; set; }
        public Signature Signature { get; }

        public ChangelogDiffGeneratorFixture()
        {
            TempDir = Path.Combine(Path.GetTempPath(), $"{nameof(ChangelogDiffGenerator)}Temp");
            TempPluginsDir = Path.Combine(TempDir, "plugins");
            TempGitDir = Path.Combine(TempDir, ".git");
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            Signature = new Signature(new Identity("TestName", "TestEmail"), DateTime.Now);
        }

        // Deletes entire temp directory, recreates it and inits git repository
        public void ResetTempDir()
        {
            Directory.SetCurrentDirectory("\\");

            if (Directory.Exists(TempDir))
            {
                if (Directory.Exists(TempGitDir))
                {
                    string[] gitFiles = Directory.GetFiles(TempGitDir, "*", SearchOption.AllDirectories);
                    foreach (string file in gitFiles)
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                    }
                }

                Directory.Delete(TempDir, true);
            }
            Directory.CreateDirectory(TempDir);
            Directory.CreateDirectory(TempPluginsDir);

            Repository.Init(TempDir);

            Repository = new Repository(TempDir);

            Directory.SetCurrentDirectory(TempDir);
        }

        public PipelineContext CreatePipelineContext()
        {
            Dictionary<string, object> sharedData = new Dictionary<string, object>();
            LinkedList<Step> steps = new LinkedList<Step>();
            DefaultProcessManager processManager = new DefaultProcessManager();

            return new PipelineContext
            {
                Repository = Repository,
                ProcessManager = processManager,
                SharedData = sharedData,
                Steps = steps
            };
        }

        public StepContext CreateStepContext(ILogger logger, IPluginOptions options)
        {
            return new StepContext
            {
                Logger = logger,
                Options = options
            };
        }

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}

