namespace JeremyTCD.PipelinesCE.Core
{
    public interface IPlugin
    {
        void Run(IPipelineContext pipelineContext, IStepContext stepContext);
    }
}
