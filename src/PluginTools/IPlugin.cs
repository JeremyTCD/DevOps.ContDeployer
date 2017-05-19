namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IPlugin
    {
        void Run(IPipelineContext pipelineContext, IStepContext stepContext);
    }
}
