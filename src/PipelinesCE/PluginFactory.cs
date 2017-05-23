using JeremyTCD.PipelinesCE.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE
{
    public class PluginFactory : IPluginFactory
    {
        private IAssemblyService _assemblyService { get; }
        private ILogger<PluginFactory> _logger { get; }
        private IContainer _mainContainer { get; }
        private IDictionary<string, IContainer> _pluginContainers { get; }

        public PluginFactory(ILogger<PluginFactory> logger,
            IContainer mainContainer,
            IDictionary<string, IContainer> pluginContainers)
        {
            _logger = logger;
            _mainContainer = mainContainer;
            _pluginContainers = pluginContainers;
        }

        public IPlugin Build(Type pluginType)
        {
            string pluginName = pluginType.Name;

            _pluginContainers.TryGetValue(pluginName, out IContainer pluginContainer);

            IPlugin plugin = (pluginContainer != null ? pluginContainer.GetInstance(pluginType) : 
                _mainContainer.GetInstance(pluginType)) as IPlugin;

            if(plugin == null)
            {
                throw new Exception($"No service for plugin type with name \"{pluginName}\"");
            }

            _logger.LogInformation($"Plugin \"{pluginName}\" successfully built");

            return plugin;
        }
    }
}
