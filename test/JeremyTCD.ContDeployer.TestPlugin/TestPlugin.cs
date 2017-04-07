using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Composition;

namespace JeremyTCD.ContDeployer.TestPlugin
{
    [Export(typeof(IPlugin))]
    public class TestPlugin : PluginBase
    {
        public override IDictionary<string, object> DefaultConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Run(IDictionary<string, object> config, PipelineContext context, ILogger logger, LinkedList<PipelineStep> steps)
        {
            throw new NotImplementedException();
        }
    }
}
