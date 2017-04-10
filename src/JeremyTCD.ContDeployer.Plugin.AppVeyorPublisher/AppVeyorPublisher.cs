using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.AppVeyorPublisher
{
    [Export(typeof(IPlugin))]
    public class AppVeyorPublisher : PluginBase
    {
        public override IDictionary<string, object> DefaultConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Run(IDictionary<string, object> config, ILogger logger, LinkedList<PipelineStep> steps)
        {
            throw new NotImplementedException();
        }
    }
}
