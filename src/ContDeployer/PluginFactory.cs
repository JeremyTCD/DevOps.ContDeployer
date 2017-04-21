using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.ContDeployer
{
    public class PluginFactory : IPluginFactory
    {
        private IAssemblyService _assemblyService { get; }
        private ILogger<PluginFactory> _logger { get; }
        private IContainer _mainContainer { get; }
        private IDictionary<string, IContainer> _pluginContainers { get; }

        private string _pluginName { get; set; }
        private Dictionary<string, Type> _pluginTypes { get; set; }

        public PluginFactory(IAssemblyService assemblyService,
            ILogger<PluginFactory> logger,
            IContainer mainContainer,
            IDictionary<string, IContainer> pluginContainers)
        {
            _assemblyService = assemblyService;
            _logger = logger;
            _mainContainer = mainContainer;
            _pluginContainers = pluginContainers;
        }

        public void LoadTypes()
        {
            IEnumerable<Assembly> pluginAssemblies = _assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);

            _pluginTypes = _assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPlugin)).ToDictionary(type => type.Name);
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

            _pluginContainers.TryGetValue(_pluginName, out IContainer pluginContainer);
            IPlugin plugin = (pluginContainer != null ? pluginContainer.GetInstance(pluginType) : 
                _mainContainer.GetInstance(pluginType)) as IPlugin;

            if(plugin == null)
            {
                throw new Exception($"No service for plugin type with name \"{_pluginName}\"");
            }

            _logger.LogInformation($"Plugin \"{_pluginName}\" successfully built");

            return plugin;
        }
    }
}
