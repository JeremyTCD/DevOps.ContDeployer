using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.Plugin.AppVeyorPublisher
{
    public class AppVeyorPublisher : PluginBase
    {
        public AppVeyorPublisher(IPipelineContext pipelineContext, IStepContext stepContext) : base(pipelineContext, stepContext)
        {
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
