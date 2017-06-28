using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using JeremyTCD.PipelinesCE.Tests.StubPluginProject;

namespace PipelinesCE.StubPluginProject
{
    public class StubPlugin : IPlugin
    {
        public StubPlugin(IStubService stubService)
        {
        }

        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
        }
    }
}
