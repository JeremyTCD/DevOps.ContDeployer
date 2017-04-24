using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.TestPlugin
{
    public class TestPlugin : PluginBase
    {
        public TestPlugin(IPipelineContext pipelineContext, IStepContext stepContext) : base(pipelineContext, stepContext)
        {
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
