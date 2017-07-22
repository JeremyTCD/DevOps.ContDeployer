namespace JeremyTCD.PipelinesCE.Tools
{
    public interface IPlugin
    {
        void Run(IPipelineContext pipelineContext, IStepContext stepContext);
    }
}
