using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System.Net.Http;

namespace JeremyTCD.PipelinesCE
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPipelinesCE(this IContainer main)
        {
            ServiceCollection services = new ServiceCollection();
            AssemblyService assemblyService = new AssemblyService();

            services.
                AddLogging().
                AddOptions().
                AddSingleton<IAssemblyService>(assemblyService).
                AddSingleton<HttpClient>().
                AddSingleton(main);

            services.
                AddSingleton<PipelinesCE>().
                AddSingleton<IProcessManager, ProcessManager>().
                AddSingleton<IPluginFactory, PluginFactory>().
                AddSingleton<IPipeline, Pipeline>().
                AddSingleton<IPipelineContext, PipelineContext>();

            //// Load assemblies in plugins directory
            //assemblyService.LoadAssembliesInDir(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), true);
            //IEnumerable<Assembly> pluginAssemblies = assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);

            //// Plugins
            //List<Type> pluginTypes = assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPlugin)).ToList();
            //foreach (Type pluginType in pluginTypes)
            //{
            //    services.AddTransient(pluginType);
            //}

            // Plugin containers
            //List<Type> pluginStartupTypes = assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPluginStartup)).ToList();
            //Dictionary<string, IContainer> pluginContainers = new Dictionary<string, IContainer>();
            //foreach (Type pluginStartupType in pluginStartupTypes)
            //{
            //    IPluginStartup pluginStartup = (IPluginStartup)Activator.CreateInstance(pluginStartupType);
            //    ServiceCollection pluginServices = new ServiceCollection();
            //    pluginStartup.ConfigureServices(pluginServices);

            //    IContainer child = main.CreateChildContainer();
            //    child.Configure(config => config.Populate(pluginServices));
            //    pluginContainers.Add(pluginStartupType.Name.Replace("Startup", ""), child);
            //}
            //services.AddSingleton<IDictionary<string, IContainer>>(pluginContainers);

            main.Populate(services);
        }
    }
}
