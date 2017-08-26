namespace JeremyTCD.PipelinesCE.Core
{
    public interface IPipelineRunner
    {
        void Run(Pipeline pipeline, IPipelineContext pipelineContext);
    }
}
