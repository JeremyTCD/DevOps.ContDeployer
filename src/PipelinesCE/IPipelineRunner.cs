using JeremyTCD.PipelinesCE.PluginTools;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE
{
    public interface IPipelineRunner 
    {
        void Run(IEnumerable<IStep> steps, PipelineOptions pipelineOptions);
    }
}
