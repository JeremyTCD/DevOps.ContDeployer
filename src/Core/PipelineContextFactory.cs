using System.Collections.Generic;
using StructureMap;

namespace JeremyTCD.PipelinesCE.Core
{
    public class PipelineContextFactory : IPipelineContextFactory
    {
        public IDictionary<string, IContainer> _pluginContainers { get; private set; }
        private SharedPluginOptions _sharedPluginOptions { get; set; }
        private PipelinesCEOptions _pipelinesCEOptions { get; set; }

        public IPipelineContextFactory AddPipelinesCEOptions(PipelinesCEOptions pipelinesCEOptions)
        {
            _pipelinesCEOptions = pipelinesCEOptions;
            return this;
        }

        public IPipelineContextFactory AddPluginContainers(IDictionary<string, IContainer> pluginContainers)
        {
            _pluginContainers = pluginContainers;
            return this;
        }

        public IPipelineContextFactory AddSharedPluginOptions(SharedPluginOptions sharedPluginOptions)
        {
            _sharedPluginOptions = sharedPluginOptions;
            return this;
        }

        public IPipelineContext CreatePipelineContext()
        {
            return new PipelineContext
            {
                PluginContainers = _pluginContainers,
                PipelinesCEOptions = _pipelinesCEOptions,
                SharedPluginOptions = _sharedPluginOptions,
                SharedData = new Dictionary<string, object>()
            };
        }
    }
}
