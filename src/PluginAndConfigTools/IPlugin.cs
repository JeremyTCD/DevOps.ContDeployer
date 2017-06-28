namespace JeremyTCD.PipelinesCE.PluginAndConfigTools
{
    public interface IPlugin
    {
        void Run(IPipelineContext pipelineContext, IStepContext stepContext);
    }
}
