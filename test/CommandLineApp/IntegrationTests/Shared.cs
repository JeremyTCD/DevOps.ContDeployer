using JeremyTCD.PipelinesCE.CommandLineApp;
using Microsoft.Extensions.DependencyInjection;
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
            IServiceCollection services = new ServiceCollection();
            startup.ConfigureServices(services);

            // Wrap services in a StructureMap container to utilize its multi-tenancy features
            IContainer mainContainer = new Container();
            mainContainer.Populate(services);

            // TODO this should be configured by PipelinesCE
            //main.GetInstance<ILoggerFactory>().AddDebug();

            return mainContainer;
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

