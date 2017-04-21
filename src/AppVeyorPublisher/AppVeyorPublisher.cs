using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.Plugin.AppVeyorPublisher
{
    public class AppVeyorPublisher : PluginBase
    {
        public AppVeyorPublisher(PipelineContext pipelineContext, StepContext stepContext) : base(pipelineContext, stepContext)
        {
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
