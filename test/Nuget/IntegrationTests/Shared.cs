using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.Nuget.Tests.IntegrationTests
{
    [CollectionDefinition(nameof(NugetClientCollection))]
    public class NugetClientCollection : ICollectionFixture<NugetClientFixture>
    {
    }

    public class NugetClientFixture
    {
        public string TempDir { get; }

        public NugetClientFixture()
        {
            TempDir = Path.Combine(Path.GetTempPath(), "NugetClientTemp");
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

