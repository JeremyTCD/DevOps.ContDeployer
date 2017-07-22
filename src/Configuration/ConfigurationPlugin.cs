using JeremyTCD.PipelinesCE.Core;

namespace JeremyTCD.PipelinesCE.Plugin.Configuration
{
    public class ConfigurationPlugin : IPlugin
    {
        /// <summary>
        /// Specifies default values for <see cref="IPipelineContext.PipelineOptions"/>
        /// </summary>
        /// <param name="pipelineContext"></param>
        /// <param name="stepContext"></param>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            // Merge stepContext.PluginOptions.PipelineOptions and pipelineContext.PipelineOptions
        }
    }
}
