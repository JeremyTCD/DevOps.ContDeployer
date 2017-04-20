using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IPluginStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}
