using JeremyTCD.PipelinesCE.ConsoleApplication;
using Microsoft.Extensions.Logging;
using StructureMap;
using System.IO;
using Xunit;

namespace JeremyTCD.PipelinesCE.Tests.IntegrationTests
{
    [CollectionDefinition(nameof(PipelinesCECollection))]
    public class PipelinesCECollection : ICollectionFixture<PipelinesCEFixture>
    {
    }

    public class PipelinesCEFixture 
    {
        public string TempDir { get; }

        public PipelinesCEFixture()
        {
            TempDir = Path.Combine(Path.GetTempPath(), $"{nameof(PipelinesCE)}Temp");
        }

        public IContainer GetContainer()
        {
            Startup startup = new Startup();
            IContainer main = new Container();
            startup.ConfigureServices(main);

            // TODO this should be configured by PipelinesCE
            //main.GetInstance<ILoggerFactory>().AddDebug();

            return main;
        }

        // Deletes entire temp directory, recreates it and inits git repository
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

