using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.MSBuild.Tests.IntegrationTests
{
    [CollectionDefinition(nameof(MSBuildClientCollection))]
    public class MSBuildClientCollection : ICollectionFixture<MSBuildClientFixture>
    {
    }

    public class MSBuildClientFixture
    {
        public string TempDir { get; }

        public MSBuildClientFixture()
        {
            TempDir = Path.Combine(Path.GetTempPath(), "MSBuildClientTemp");
        }

        // Deletes entire temp directory then recreates it
        public void ResetTempDir()
        {
            Directory.SetCurrentDirectory("\\");

            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }
            Directory.CreateDirectory(TempDir);

            Directory.SetCurrentDirectory(TempDir);
        }
    }
}

