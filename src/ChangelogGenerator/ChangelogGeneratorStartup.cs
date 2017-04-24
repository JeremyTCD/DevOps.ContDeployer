using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public class ChangelogGeneratorStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IChangelogFactory, ChangelogFactory>();
        }
    }
}
