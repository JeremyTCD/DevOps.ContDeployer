using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Composition;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    [Export(typeof(IPlugin))]
    public class TagGenerator : PluginBase
    {
        public override IDictionary<string, object> DefaultConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Run(IDictionary<string, object> config, PipelineContext context, LinkedList<PipelineStep> steps)
        {
            throw new NotImplementedException();
        }
    }
}
