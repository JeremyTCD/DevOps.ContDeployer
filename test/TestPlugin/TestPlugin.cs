using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.TestPlugin
{
    public class TestPlugin : PluginBase
    {
        public TestPlugin(PipelineContext pipelineContext, StepContext stepContext) : base(pipelineContext, stepContext)
        {
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
