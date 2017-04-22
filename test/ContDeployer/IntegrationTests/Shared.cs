using LibGit2Sharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Tests.IntegrationTests
{
    [CollectionDefinition(nameof(ContDeployerCollection))]
    public class ContDeployerCollection : ICollectionFixture<ContDeployerFixture>
    {
    }

    public class ContDeployerFixture 
    {
        public string TempDir { get; }
        public string TempPluginsDir { get; }
        public JsonSerializerSettings SerializerSettings { get; }
        public string TempGitDir { get; }
        public Signature Signature { get; }

        public ContDeployerFixture()
        {
            TempDir = Path.Combine(Path.GetTempPath(), "ContDeployerTemp");
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
                    string[] gitFiles = Directory.GetFiles(Path.Combine(TempDir, ".git"), "*", SearchOption.AllDirectories);
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

            Directory.SetCurrentDirectory(TempDir);
        }
    }
}

