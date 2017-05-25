using JeremyTCD.PipelinesCE.PluginTools;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE
{
    public interface IPipeline 
    {
        void Run(IEnumerable<IStep> steps, PipelineOptions pipelineOptions);
    }
}
