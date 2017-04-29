using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Common;

namespace JeremyTCD.ContDeployer.Plugin.Nuget
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
