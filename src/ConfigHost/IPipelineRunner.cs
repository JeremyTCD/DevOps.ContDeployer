using JeremyTCD.PipelinesCE.Tools;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.ConfigHost
{
    public interface IPipelineRunner 
    {
        void Run(Pipeline pipeline, IDictionary<string, IContainer> pluginContainers);
    }
}
