using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.PluginAndConfigTools
{
    public interface IPluginStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}
