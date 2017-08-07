using JeremyTCD.DotNetCore.ProjectHost;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public static class CommandLineAppServiceCollectionExtensions
    {
        public static void AddCommandLineApp(this IServiceCollection services)
        {
            services.AddProjectHost();
            services.AddCore();

            services.AddSingleton<RunCommand>();
            services.AddSingleton<RootCommand>();
        }
    }
}
