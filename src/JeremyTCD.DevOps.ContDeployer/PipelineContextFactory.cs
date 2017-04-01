using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;
using System.Text;
using System.Reflection;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.DevOps.ContDeployer
{
    public class PipelineContextFactory
    {
        public ILogger<PipelineContextFactory> Logger { get; }

        public PipelineContextFactory(ILogger<PipelineContextFactory> logger)
        {
            Logger = logger;
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

            // Get and load assemblies 
            Logger.LogInformation("Loading assemblies");
            string cwd = Directory.GetCurrentDirectory();
            List<Assembly> assemblies = Directory.
                GetFiles(Path.Combine(cwd, "plugins"), "*.dll", SearchOption.AllDirectories).
                Select(path =>
                {
                    Assembly assembly = null;
                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                        Logger.LogInformation($"Loaded assembly: {path}");
                    }
                    catch 
                    {
                        Logger.LogWarning($"Unable to load assembly: {path}. It may already be loaded.");
                    }

                    return assembly;
                }).
                Where(assembly => assembly != null).
                ToList();
            assemblies.
                Add(typeof(PipelineContextFactory).GetTypeInfo().Assembly);

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