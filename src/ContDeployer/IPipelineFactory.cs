using JeremyTCD.ContDeployer.PluginTools;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer
{
    /// <summary>
    /// TODO tweak to allow for premade pipelines
    /// </summary>
    public interface IPipelineFactory
    {
        IEnumerable<IStep> Build();
    }
}
