using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public interface IPluginStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}
