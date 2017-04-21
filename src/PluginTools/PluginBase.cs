namespace JeremyTCD.ContDeployer.PluginTools
{
    public abstract class PluginBase : IPlugin
    {
        protected PipelineContext PipelineContext { get; }
        protected StepContext StepContext { get; }

        public PluginBase(PipelineContext pipelineContext, StepContext stepContext)
        {
            PipelineContext = pipelineContext;
            StepContext = stepContext;
        }

        public abstract void Run();
    }
}
