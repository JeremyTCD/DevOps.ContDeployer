using StructureMap;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace JeremyTCD.PipelinesCE.Core
{
    public interface IPipelineContextBuilder
    {
        IPipelineContextBuilder AddSharedStepOptions(SharedStepOptions sharedStepOptions);
        IPipelineContextBuilder AddPipelinesCEOptions(PipelinesCEOptions pipelinesCEOptions);
        IPipelineContextBuilder AddPluginContainers(IDictionary<string, IContainer> pluginContainers);
        IPipelineContextBuilder AddLoggerFactory(ILoggerFactory loggerFactory);
        IPipelineContext BuildPipelineContext();
    }
}
