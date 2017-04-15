namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IPlugin
    {
        void Run(PipelineContext pipelineContext, StepContext stepContext);
    }
}
