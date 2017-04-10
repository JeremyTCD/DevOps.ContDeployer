using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public abstract class PluginBase : IPlugin
    {
        public IRepository Repository { get; set; }

        public PluginBase(IRepository repository)
        {
            Repository = repository;
        }

        public abstract void Run(LinkedList<PipelineStep> steps);
    }
}
