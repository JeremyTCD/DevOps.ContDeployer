using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class Startup
    { 
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPipelinesCE();
            services.AddSingleton<RootCommand>();
            services.AddSingleton<RunCommand>();
            services.AddSingleton<ICommandLineUtilsService, CommandLineUtilsService>();
            services.AddOptions();
        }
    }
}
