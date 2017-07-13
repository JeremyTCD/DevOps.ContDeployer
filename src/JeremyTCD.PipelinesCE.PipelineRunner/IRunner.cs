using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE
{
    public interface IRunner 
    {
        void Run(Pipeline pipeline, IDictionary<string, IContainer> pluginContainers);
    }
}
