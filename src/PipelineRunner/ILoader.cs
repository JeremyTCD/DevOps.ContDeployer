using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.PipelineRunner
{
    public interface ILoader
    {
        (Pipeline, IDictionary<string, IContainer>) Load(PipelineOptions pipelineOptions);
    }
}