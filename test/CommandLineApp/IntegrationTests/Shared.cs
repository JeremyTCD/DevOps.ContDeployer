using JeremyTCD.PipelinesCE.CommandLineApp;
using Microsoft.Extensions.Logging;
using StructureMap;
using System.IO;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    [CollectionDefinition(nameof(CommandLineAppCollection))]
    public class CommandLineAppCollection : ICollectionFixture<CommandLineAppFixture>
    {
    }

    public class CommandLineAppFixture 
    {
        public string TempDir { get; }

        public CommandLineAppFixture()
        {
            TempDir = Path.Combine(Path.GetTempPath(), $"{nameof(CommandLineApp)}Temp");
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

