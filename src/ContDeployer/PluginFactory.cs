using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.ContDeployer
{
    public class PluginFactory : IPluginFactory
    {
        private IAssemblyService _assemblyService { get; }
        private Dictionary<string, Type> _pluginTypes { get; set; }
        private ILogger<PluginFactory> _logger { get; }
        private string _pluginName { get; set; }

        public PluginFactory(IAssemblyService assemblyService,
            ILogger<PluginFactory> logger)
        {
            _assemblyService = assemblyService;
            _logger = logger;
        }

        public void LoadTypes()
        {
            AssemblyService assemblyService = new AssemblyService();
            assemblyService.LoadAssembliesInDir(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), true);
            IEnumerable<Assembly> pluginAssemblies = assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);

            _pluginTypes = assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPlugin)).ToDictionary(type => type.Name);
        }

        public IPluginFactory SetPluginName (string pluginName)
        {
            _pluginName = pluginName;

            return this;
        }

        public IPlugin Build()
        {
            _pluginTypes.TryGetValue(_pluginName, out Type pluginType);

            if(pluginType == null)
            {
                throw new Exception($"Plugin type with name \"{_pluginName}\" does not exist");
            }

            IPlugin plugin = Activator.CreateInstance(pluginType) as IPlugin;

            _logger.LogInformation($"Plugin \"{_pluginName}\" successfully built");

            return plugin;
        }
    }
}
