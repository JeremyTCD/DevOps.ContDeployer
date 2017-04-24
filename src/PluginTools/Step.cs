using Microsoft.Extensions.Configuration;

namespace JeremyTCD.ContDeployer.PluginTools
{
    /// <summary>
    /// A step in the pipeline. 
    /// </summary>
    public class Step : IStep
    {
        public Step()
        {

        }

        public Step(string pluginName, IPluginOptions options)
        {
            PluginName = pluginName;
            Options = options;
        }

        /// <summary>
        /// Name of plugin
        /// </summary>
        public string PluginName { get; set; }

        /// <summary>
        /// Raw config
        /// </summary>
        public IConfigurationSection Config { get; set; } 

        /// <summary>
        /// Instantiated options
        /// </summary>
        public IPluginOptions Options { get; }
    }
}
