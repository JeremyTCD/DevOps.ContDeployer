using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Composition.Hosting;
using System.Linq;
using Microsoft.Extensions.Logging;
using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;

namespace JeremyTCD.ContDeployer
{
    public class PipelineContextFactory
    {
        public ILogger<PipelineContextFactory> Logger { get; }
        public IAssemblyService AssemblyService { get; }

        public PipelineContextFactory(ILogger<PipelineContextFactory> logger, IAssemblyService assemblyService)
        {
            Logger = logger;
            AssemblyService = assemblyService;
        }

        /// <summary>
        /// Creates a <see cref="PipelineContext"/> instance
        /// </summary>
        /// <returns>
        /// <see cref="PipelineContext"/>
        /// </returns>
        public PipelineContext Build()
        {
            Logger.LogInformation("=== Building pipeline context ===");

            // Get assemblies 
            Logger.LogInformation("Loading assemblies");
            string pluginToolsAssemblyName = typeof(IPlugin).GetTypeInfo().Assembly.GetName().Name;
            string currentWorkingDir = Directory.GetCurrentDirectory();
            IEnumerable<Assembly> assemblies = AssemblyService.
                GetReferencingAssemblies(pluginToolsAssemblyName).
                Concat(AssemblyService.GetAssembliesInDir(Path.Combine(currentWorkingDir, "plugins"), true));

            // Get instances of types that implement IPlugin
            Logger.LogInformation("Instantiating plugins");
            ContainerConfiguration configuration = new ContainerConfiguration().
                WithAssemblies(assemblies);
            CompositionHost host = configuration.CreateContainer();
            IDictionary<string, IPlugin> plugins = host.GetExports<IPlugin>().
                ToDictionary(plugin => plugin.GetType().Name);

            // Instantiate pipeline context
            PipelineContext context = new PipelineContext(plugins);

            Logger.LogInformation("=== Pipeline context successfully built ===");
            return context;
        }
    }
}