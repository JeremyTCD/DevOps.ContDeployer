using JeremyTCD.PipelinesCE.PluginTools;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE
{
    /// <summary>
    /// TODO tweak to allow for premade pipelines
    /// </summary>
    public interface IPipelineFactory
    {
        IEnumerable<IStep> CreatePipeline();
    }
}
