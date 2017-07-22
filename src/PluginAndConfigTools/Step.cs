using System;

namespace JeremyTCD.PipelinesCE.Tools
{
    public class Step<T> : IStep where T : IPlugin
    {

        public Step(IPluginOptions options)
        {
            PluginType = typeof(T);
            PluginOptions = options;
        }

        /// <summary>
        /// Name of plugin
        /// </summary>
        public Type PluginType { get; set; }

        /// <summary>
        /// Instantiated options
        /// </summary>
        public IPluginOptions PluginOptions { get; set; }
    }
}
