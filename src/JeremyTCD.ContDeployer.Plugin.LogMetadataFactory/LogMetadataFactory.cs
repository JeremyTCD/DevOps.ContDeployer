using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Composition;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.LogMetadataFactory
{
    [Export(typeof(IPlugin))]
    public class LogMetadataFactory : IPlugin
    {
        public void Execute(IDictionary<string, object> config, PipelineContext context, LinkedList<PipelineStep> steps)
        {
            throw new NotImplementedException();
        }
    }
}
