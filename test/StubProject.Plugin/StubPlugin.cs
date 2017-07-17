using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using JeremyTCD.PipelinesCE.Tests.StubPluginProject;
using Newtonsoft.Json;

namespace PipelinesCE.StubPluginProject
{
    public class StubPlugin : IPlugin
    {
        public StubPlugin(IStubService stubService)
        {
        }

        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            // Use NewtonSoft Json as a test external dependency of the project to verify that 
            // external assemblies are loaded properly
            string test = JsonConvert.SerializeObject(new { test = "test" });
        }
    }
}
