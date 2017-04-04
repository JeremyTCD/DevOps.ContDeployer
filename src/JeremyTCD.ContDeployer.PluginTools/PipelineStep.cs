using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public class PipelineStep
    {
        public string PluginName { get; set; }
        public IDictionary<string, object> Config { get; set; }
    }
}
