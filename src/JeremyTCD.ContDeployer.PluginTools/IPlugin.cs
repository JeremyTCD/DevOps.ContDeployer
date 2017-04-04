using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IPlugin
    {
        void Execute(IDictionary<string, object> config, PipelineContext context, LinkedList<PipelineStep> steps);
    }
}
