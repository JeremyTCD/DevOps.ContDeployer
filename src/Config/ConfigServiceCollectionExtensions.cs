using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.Config
{
    public static class ConfigServiceCollectionExtensions
    {
        public static void AddConfigHost(this IServiceCollection services)
        {
            services.AddUtils();
            services.AddCore();

            services.AddSingleton<IConfigRunner, ConfigRunner>();
            services.AddSingleton<IConfigLoader, ConfigLoader>();
        }
    }
}
