using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.ContDeployer.Plugin.Changelog
{
    public class ChangelogPluginStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IChangelogFactory, ChangelogFactory>();
        }
    }
}
