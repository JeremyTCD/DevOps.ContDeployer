using System.IO;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.Nuget.Tests.IntegrationTests
{
    [CollectionDefinition(nameof(NugetCollection))]
    public class NugetCollection : ICollectionFixture<NugetFixture>
    {
    }

    public class NugetFixture
    {
        public string TempDir { get; }

        public NugetFixture()
        {
            TempDir = Path.Combine(Path.GetTempPath(), $"{nameof(NugetFixture)}Temp");
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

