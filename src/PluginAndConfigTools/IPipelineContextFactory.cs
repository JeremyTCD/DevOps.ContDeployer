namespace JeremyTCD.PipelinesCE.Tools
{
    public interface IPipelineContextFactory
    {
        IPipelineContextFactory AddPipelineOptions(PipelineOptions pipelineOptions);

        IPipelineContext CreatePipelineContext();
    }
}
