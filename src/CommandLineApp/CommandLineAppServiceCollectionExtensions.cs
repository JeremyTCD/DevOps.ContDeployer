using JeremyTCD.DotNetCore.ProjectHost;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public static class CommandLineAppServiceCollectionExtensions
    {
        public static void AddCommandLineApp(this IServiceCollection services)
        {
            services.AddProjectHost();

            services.AddSingleton<RunCommand>();
            services.AddSingleton<RootCommand>();
        }
    }
}
