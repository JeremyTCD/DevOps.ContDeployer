using JeremyTCD.PipelinesCE.PluginAndConfigTools;
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

        public void Run(PipelineOptions pipelineOptions)
        {
            // Load 
            (Pipeline pipeline, IDictionary<string, IContainer> pluginContainers) = _pipelineLoader.Load(pipelineOptions);

            // Run
            pipeline.Options = pipelineOptions.Combine(pipeline.Options);
            _pipelineRunner.Run(pipeline, pluginContainers);
        }
    }
}
