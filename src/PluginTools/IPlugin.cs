namespace JeremyTCD.PipelinesCE.PluginTools
{
    public interface IPlugin
    {
        void Run(IPipelineContext pipelineContext, IStepContext stepContext);
    }
}
