using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.DevOps.ContDeployer
{
    public class PluginConfig
    {
        public string Name { get; set; }
        public Dictionary<string, object> Config { get; set; }
    }
}
