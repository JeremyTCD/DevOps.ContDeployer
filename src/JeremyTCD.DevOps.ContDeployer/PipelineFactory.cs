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

namespace JeremyTCD.DevOps.ContDeployer
{
    public class PipelineFactory
    {
        public PipelineFactoryOptions Options { get; }

        [ImportMany]
        public IEnumerable<IPlugin> Plugins { get; set; }

        public PipelineFactory(IOptions<PipelineFactoryOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        // Test with plugins in a plugins folder, use SetCurrentDirectory
        //      what if they have dependencies?
        public Pipeline CreatePipeline()
        {
            Assembly assembly = typeof(PipelineFactory).GetTypeInfo().Assembly;
            // Get assemblies in plugins folder
            IEnumerable<Assembly> assemblies = Directory
                .GetFiles("plugins", "*.dll", SearchOption.AllDirectories)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                .ToList();

            ContainerConfiguration configuration = new ContainerConfiguration().
                //WithAssemblies(assemblies);
                WithAssembly(assembly);
            CompositionHost host = configuration.CreateContainer();
            host.SatisfyImports(this);
            IDictionary<string, IPlugin> pluginsHashTable = Plugins.ToDictionary(plugin => plugin.GetType().Name);

            // Sort plugins according to options order
            List<IPlugin> pipelinePlugins = new List<IPlugin>();
            foreach (PluginConfig config in Options.PluginConfigs)
            {

                IPlugin plugin;
                if (pluginsHashTable.TryGetValue(config.Name, out plugin))
                {
                    pipelinePlugins.Add(plugin);
                }
                else
                {
                    throw new Exception($"{nameof(PipelineFactory)}: No plugin with name {config.Name} exists");
                }
            }

            return new Pipeline(pipelinePlugins, Options.PluginConfigs);
        }
    }
}