using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;

namespace JeremyTCD.PipelinesCE
{
    public class PluginFactory : IPluginFactory
    {
        private ILogger<PluginFactory> _logger { get; }
        private IContainer _mainContainer { get; }

        public PluginFactory(ILogger<PluginFactory> logger,
            IContainer mainContainer)
        {
            _logger = logger;
            _mainContainer = mainContainer;
        }

        public IPlugin CreatePlugin(Type pluginType)
        {
            string pluginName = pluginType.Name;

            IContainer container = _mainContainer.GetProfile(pluginName);

            if(container == null)
            {
                throw new Exception($"No container for plugin type with name \"{pluginName}\"");
            }

            IPlugin plugin = container.GetInstance(pluginType) as IPlugin;

            if(plugin == null)
            {
                throw new Exception($"No service for plugin type with name \"{pluginName}\"");
            }

            _logger.LogInformation($"Plugin \"{pluginName}\" successfully built");

            return plugin;
        }
    }
}
