using JeremyTCD.PipelinesCE.Core;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.ConfigHost
{
    public interface IPipelineLoader
    {
        (Pipeline, IDictionary<string, IContainer>) Load(PipelineOptions pipelineOptions);
    }
}