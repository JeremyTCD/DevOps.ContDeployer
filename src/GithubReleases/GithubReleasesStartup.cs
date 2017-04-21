using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleases
{
    public class GithubReleasesStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new GitHubClient(new ProductHeaderValue(nameof(ContDeployer))));
        }
    }
}
