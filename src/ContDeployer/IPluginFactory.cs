using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer
{
    public interface IPluginFactory
    {
        IPlugin Build(Type pluginType);
    }
}
