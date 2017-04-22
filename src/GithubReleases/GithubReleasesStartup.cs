using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleases
{
    public class GithubReleasesStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new GithubClientFactory("https://api.github.com", nameof(ContDeployer)));
        }
    }
}
