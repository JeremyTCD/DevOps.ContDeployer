using JeremyTCD.PipelinesCE.PluginTools;
using System;

namespace JeremyTCD.PipelinesCE
{
    public interface IPluginFactory
    {
        IPlugin CreatePlugin(Type pluginType);
    }
}
