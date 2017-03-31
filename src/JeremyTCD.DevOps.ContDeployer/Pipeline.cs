using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace JeremyTCD.DevOps.ContDeployer
{
    public class Pipeline
    {
        public List<IPlugin> Plugins { get; set; }
        public List<PluginConfig> Configs { get; set; }

        public Pipeline(List<IPlugin> plugins, List<PluginConfig> configs)
        {
            Plugins = plugins;
            Configs = configs;
        }

        public void Run()
        {
            foreach(IPlugin plugin in Plugins)
            {
                plugin.Execute(/*metadata, config, allconfigs*/);
            }
        }
    }
}
