using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.PipelineRunner
{
    public class Root
    {
        private ILoader _loader { get; }
        private IRunner _runner { get; }

        public Root(ILoader loader, IRunner runner)
        {
            _loader = loader;
            _runner = runner;
        }

        public void Run(PipelineOptions pipelineOptions)
        {
            // Load 
            (Pipeline pipeline, IDictionary<string, IContainer> pluginContainers) = _loader.Load(pipelineOptions);

            // Run
            pipeline.Options = pipelineOptions.Combine(pipeline.Options);
            _runner.Run(pipeline, pluginContainers);
        }
    }
}
