using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Collections.Generic;
using System.Composition;

namespace JeremyTCD.ContDeployer.TestPlugin
{
    [Export(typeof(IPlugin))]
    public class TestPlugin : IPlugin
    {
        public void Run(IDictionary<string, object> config, PipelineContext context, LinkedList<PipelineStep> steps)
        {
            throw new NotImplementedException();
        }
    }
}
