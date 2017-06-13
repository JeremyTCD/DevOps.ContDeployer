using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using StructureMap;
using System;

namespace JeremyTCD.PipelinesCE
{
    public class PluginFactory : IPluginFactory
    {
        private ILoggingService<PluginFactory> _loggingService { get; }
        private IContainer _mainContainer { get; }

        public PluginFactory(ILoggingService<PluginFactory> loggingService,
            IContainer mainContainer)
        {
            _loggingService = loggingService;
            _mainContainer = mainContainer;
        }

        /// <summary>
        /// Instantiates instance of type <paramref name="pluginType"/>
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns>
        /// <see cref="IPlugin"/>
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if <paramref name="pluginType"/> has no corresponding StructureMap profile
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if StructureMap container has no service for type <paramref name="pluginType"/>
        /// </exception>
        public IPlugin CreatePlugin(Type pluginType)
        {
            string pluginName = pluginType.Name;

            _loggingService.LogDebug(Strings.Log_BuildingPlugin, pluginName);

            IContainer container = _mainContainer.GetProfile(pluginName);

            if (container == null)
            {
                throw new Exception(string.Format(Strings.Exception_NoContainerForPluginType, pluginName));
            }

            IPlugin plugin = container.GetInstance(pluginType) as IPlugin;

            if (plugin == null)
            {
                throw new Exception(string.Format(Strings.Exception_NoServiceForPluginType, pluginName));
            }

            return plugin;
        }
    }
}
