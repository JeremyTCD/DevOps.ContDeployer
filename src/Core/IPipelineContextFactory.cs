using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public interface IPipelineContextFactory
    {
        IPipelineContextFactory AddSharedPluginOptions(SharedPluginOptions sharedPluginOptions);
        IPipelineContextFactory AddPipelinesCEOptions(PipelinesCEOptions pipelinesCEOptions);
        IPipelineContextFactory AddPluginContainers(IDictionary<string, IContainer> pluginContainers);
        IPipelineContext CreatePipelineContext();
    }
}
