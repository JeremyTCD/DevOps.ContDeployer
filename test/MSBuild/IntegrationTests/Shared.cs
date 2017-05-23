using System.IO;
using Xunit;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild.Tests.IntegrationTests
{
    [CollectionDefinition(nameof(MSBuildCollection))]
    public class MSBuildCollection : ICollectionFixture<MSBuildFixture>
    {
    }

    public class MSBuildFixture
    {
        public string TempDir { get; }

        public MSBuildFixture()
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

