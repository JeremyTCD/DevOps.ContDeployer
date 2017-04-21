using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleases.IntegrationTests
{
    [CollectionDefinition(nameof(GithubReleasesCollection))]
    public class GithubReleasesCollection : ICollectionFixture<GithubReleasesFixture>
    {
    }

    public class GithubReleasesFixture
    {
        public string TempDir { get; }
        public string TempPluginsDir { get; }
        public string TempGitDir { get; }
        public JsonSerializerSettings SerializerSettings { get; }
        public Signature Signature { get; }

        public GithubReleasesFixture()
        {
            TempDir = Path.Combine(Path.GetTempPath(), $"{nameof(GithubReleases)}Temp");
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

            Directory.SetCurrentDirectory(TempDir);
        }
    }
}

