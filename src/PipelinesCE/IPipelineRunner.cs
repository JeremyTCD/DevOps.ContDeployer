using JeremyTCD.PipelinesCE.PluginTools;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE
{
    public interface IPipelineRunner 
    {
        void Run(Pipeline pipeline, IDictionary<string, IContainer> pluginContainers);
    }
}
