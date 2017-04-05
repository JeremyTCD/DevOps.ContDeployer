using LibGit2Sharp;
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

        public E2EFixture()
        {
            TempDir = Path.Combine(Path.GetTempPath(), "ContDeployerTemp");
            TempPluginsDir = Path.Combine(TempDir, "plugins");
        }

        // Deletes entire temp directory, recreates it and inits git repository
        public void ResetTempDir()
        {
            // instead of setting current directory, add a directory option to cd.json
            // so devenv doesn't fuck up tests. 
            Directory.SetCurrentDirectory("\\");

            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }
            Directory.CreateDirectory(TempDir);
            Directory.CreateDirectory(TempPluginsDir);

            Repository.Init(TempDir);

            Directory.SetCurrentDirectory(TempDir);
        }

        public void Dispose()
        {
            Directory.Delete(TempDir, true);
            Directory.SetCurrentDirectory("\\");
        }
    }
}

