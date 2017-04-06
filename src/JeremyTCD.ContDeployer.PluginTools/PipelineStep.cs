using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.PluginTools
{
    /// <summary>
    /// A step in the pipeline. 
    /// </summary>
    public class PipelineStep
    {
        /// <summary>
        /// Name of plugin for the step
        /// </summary>
        public string PluginName { get; set; } = "";

        /// <summary>
        /// Configuration for the step
        /// </summary>
        public IDictionary<string, object> Config { get; set; } = new Dictionary<string, Object>();
    }
}
