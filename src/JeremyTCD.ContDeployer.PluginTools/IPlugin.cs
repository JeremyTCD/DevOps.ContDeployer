using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IPlugin
    {
        void Run(Dictionary<string, object> sharedData, LinkedList<PipelineStep> steps);
    }
}
