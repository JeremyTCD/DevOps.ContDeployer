using JeremyTCD.PipelinesCE.Tools;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Common;

namespace JeremyTCD.PipelinesCE.Plugin.Nuget
{
    public class NugetChangelogAdapterStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogger, NugetLogger>();
            services.AddSingleton<INugetClient, NugetClient>();
        }
    }
}
