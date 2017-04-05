using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public class PipelineContext
    {
        /// <summary>
        /// Hash table of all available <see cref="IPlugin"/> instances
        /// </summary>
        public IDictionary<string , IPlugin> Plugins { get; set; }

        /// <summary>
        /// Data shared by pipeline steps
        /// </summary>
        public IDictionary<string, object> GlobalData { get; set; }

        public IRepository Repository { get; set; }

        public PipelineContext(IDictionary<string, IPlugin> plugins, IRepository repository)
        {
            Plugins = plugins;
            Repository = repository;
            GlobalData = new Dictionary<string, object>();
        }
    }
}
