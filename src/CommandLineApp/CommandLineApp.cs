using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System.Linq;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class CommandLineApp
    {
        public static void Main(string[] args)
        {
            Startup startup = new Startup();
            IServiceCollection services = new ServiceCollection();
            startup.ConfigureServices(services);

            // Wrap services in a StructureMap container to utilize its multi-tenancy features
            IContainer mainContainer = new Container();
            mainContainer.Populate(services);

            RootCommand defaultCommand = mainContainer.GetInstance<RootCommand>();
            args = args.Select(s => s.ToLowerInvariant()).ToArray();
            defaultCommand.Execute(args);
        }
    }
}
 