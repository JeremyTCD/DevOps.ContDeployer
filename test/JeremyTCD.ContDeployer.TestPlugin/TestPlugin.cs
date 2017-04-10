using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Composition;

namespace JeremyTCD.ContDeployer.TestPlugin
{
    public class TestPlugin : PluginBase
    {
        public TestPlugin(IRepository repository):base(repository)
        {
        }

        public override void Run(LinkedList<PipelineStep> steps)
        {
            throw new NotImplementedException();
        }
    }
}
