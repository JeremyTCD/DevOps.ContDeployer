using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.ConfigHost
{
    public static class ConfigHostServiceCollectionExtensions
    {
        public static void AddConfigHost(this IServiceCollection services)
        {
            services.AddUtils();
            services.AddCore();

            services.AddSingleton<ConfigHostCore>();
            services.AddSingleton<IPipelineLoader, PipelineLoader>();
            services.AddSingleton<IPipelineRunner, PipelineRunner>();
        }
    }
}
