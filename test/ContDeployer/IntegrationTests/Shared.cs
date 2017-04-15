using LibGit2Sharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace JeremyTCD.ContDeployer.Tests.IntegrationTests
{
    [CollectionDefinition(nameof(E2ECollection))]
    public class E2ECollection : ICollectionFixture<E2EFixture>
    {
    }

    public class E2EFixture : IDisposable
    {
        public string TempDir { get; }
        public string TempPluginsDir { get; }
        public JsonSerializerSettings SerializerSettings { get; }
        public Repository Repository { get; set; }
        public string TempGitDir { get; }
        public Signature Signature { get; }

        public E2EFixture()
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

            Repository = new Repository(TempDir);

            Directory.SetCurrentDirectory(TempDir);
        }

        public void Dispose()
        {
            Directory.SetCurrentDirectory("\\");
            Repository.Dispose();
        }
    }
}

