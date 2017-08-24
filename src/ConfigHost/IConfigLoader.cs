using JeremyTCD.PipelinesCE.Core;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Config
{
    public interface IConfigLoader
    {
        (Pipeline, IDictionary<string, IContainer>) Load(PipelinesCEOptions pipelineOptions);
    }
}