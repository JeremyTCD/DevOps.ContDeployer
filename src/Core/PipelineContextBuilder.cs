using StructureMap;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace JeremyTCD.PipelinesCE.Core
{
    public class PipelineContextBuilder : IPipelineContextBuilder
    {
        public IDictionary<string, IContainer> _pluginContainers { get; private set; }
        private SharedStepOptions _sharedPluginOptions { get; set; }
        private PipelinesCEOptions _pipelinesCEOptions { get; set; }
        private ILoggerFactory _loggerFactory { get; set; }

        public IPipelineContextBuilder AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        public IPipelineContextBuilder AddPipelinesCEOptions(PipelinesCEOptions pipelinesCEOptions)
        {
            _pipelinesCEOptions = pipelinesCEOptions;
            return this;
        }

        public IPipelineContextBuilder AddPluginContainers(IDictionary<string, IContainer> pluginContainers)
        {
            _pluginContainers = pluginContainers;
            return this;
        }

        public IPipelineContextBuilder AddSharedStepOptions(SharedStepOptions sharedStepOptions)
        {
            _sharedPluginOptions = sharedStepOptions;
            return this;
        }

        public IPipelineContext BuildPipelineContext()
        {
            return new PipelineContext
            {
                PluginContainers = _pluginContainers,
                PipelinesCEOptions = _pipelinesCEOptions,
                SharedStepOptions = _sharedPluginOptions,
                SharedData = new Dictionary<string, object>()
            };
        }
    }
}
