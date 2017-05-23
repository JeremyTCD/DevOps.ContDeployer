using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.ContDeployer.Plugin.Git
{
    public class GitPluginStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRepositoryFactory, RepositoryFactory>();
        }
    }
}
