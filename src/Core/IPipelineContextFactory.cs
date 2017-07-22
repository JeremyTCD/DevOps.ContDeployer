namespace JeremyTCD.PipelinesCE.Core
{
    public interface IPipelineContextFactory
    {
        IPipelineContextFactory AddPipelineOptions(PipelineOptions pipelineOptions);

        IPipelineContext CreatePipelineContext();
    }
}
