using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class Startup
    { 
        public void ConfigureServices(IContainer mainContainer)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddPipelinesCE();
            services.AddSingleton<DefaultCommand>();
            services.AddSingleton<RunCommand>();

            mainContainer.Populate(services);
        }
    }
}
