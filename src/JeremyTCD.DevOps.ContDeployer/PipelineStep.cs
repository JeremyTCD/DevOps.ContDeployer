using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.DevOps.ContDeployer
{
    public class PipelineStep
    {
        public string PluginName { get; set; }
        public IDictionary<string, object> PluginConfig { get; set; }
    }
}
