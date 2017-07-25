using JeremyTCD.PipelinesCE.Core;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.ConfigHost
{
    public class ConfigHostCore
    {
        private IPipelineLoader _pipelineLoader { get; }
        private IPipelineRunner _pipelineRunner { get; }

        public ConfigHostCore(IPipelineLoader pipelineLoader, IPipelineRunner pipelineRunner)
        {
            _pipelineLoader = pipelineLoader;
            _pipelineRunner = pipelineRunner;
        }

        public void Run(PipelinesCEOptions pipelinesCEOptions, SharedPluginOptions sharedPluginOptions)
        {
            // Load 
            (Pipeline pipeline, IDictionary<string, IContainer> pluginContainers) = _pipelineLoader.Load(pipelinesCEOptions);

            // Run
            pipeline.Options = sharedPluginOptions.Combine(pipeline.Options);
            _pipelineRunner.Run(pipeline, pluginContainers);
        }
    }
}
