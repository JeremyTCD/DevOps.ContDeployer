using LibGit2Sharp;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public abstract class PluginBase : IPlugin
    {
        public abstract void Run(PipelineContext pipelineContext, PipelineStepContext pipelineStepContext);
    }
}
