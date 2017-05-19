using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.Plugin.AppVeyorPlugin
{
    public class AppVeyorPlugin : PluginBase
    {
        public AppVeyorPlugin(IPipelineContext pipelineContext, IStepContext stepContext) : base(pipelineContext, stepContext)
        {
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
