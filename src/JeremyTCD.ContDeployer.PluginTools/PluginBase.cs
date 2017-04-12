using LibGit2Sharp;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public abstract class PluginBase : IPlugin
    {
        public IRepository Repository { get; set; }

        public PluginBase(IRepository repository)
        {
            Repository = repository;
        }

        public abstract void Run(Dictionary<string, object> sharedData, LinkedList<PipelineStep> steps);
    }
}
