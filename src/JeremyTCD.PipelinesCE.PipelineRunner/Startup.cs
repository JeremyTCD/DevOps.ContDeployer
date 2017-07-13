using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.Logging;
using StructureMap;

namespace JeremyTCD.PipelinesCE.PipelineRunner
{
    public class Startup
    {
        public void Configure(IContainer container, PipelineOptions pipelineOptions)
        {
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
            
            // TODO configure
        }
    }
}