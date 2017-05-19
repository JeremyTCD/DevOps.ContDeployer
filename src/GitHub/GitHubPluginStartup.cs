using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.ContDeployer.Plugin.GitHub
{
    public class GitHubPluginStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new GitHubClientFactory("https://api.gitHub.com", nameof(ContDeployer)));
        }
    }
}
