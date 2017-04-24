namespace JeremyTCD.ContDeployer.PluginTools
{
    public abstract class PluginBase : IPlugin
    {
        protected IPipelineContext PipelineContext { get; }
        protected IStepContext StepContext { get; }

        public PluginBase(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            PipelineContext = pipelineContext;
            StepContext = stepContext;
        }

        public abstract void Run();
    }
}
