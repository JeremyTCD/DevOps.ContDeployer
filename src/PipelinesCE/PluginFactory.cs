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

        public IPlugin CreatePlugin(Type pluginType)
        {
            string pluginName = pluginType.Name;

            IContainer container = _mainContainer.GetProfile(pluginName);

            if(container == null)
            {
                throw new Exception(string.Format(Strings.Exception_NoContainerForPluginType, pluginName));
            }

            IPlugin plugin = container.GetInstance(pluginType) as IPlugin;

            if(plugin == null)
            {
                throw new Exception(string.Format(Strings.Exception_NoServiceForPluginType, pluginName));
            }

            _loggingService.LogInformation(string.Format(Strings.Log_PluginSuccessfullyBuilt, pluginName));

            return plugin;
        }
    }
}
