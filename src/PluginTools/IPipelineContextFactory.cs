namespace JeremyTCD.PipelinesCE.PluginTools
{
    public interface IPipelineContextFactory
    {
        IPipelineContextFactory AddPipelineOptions(PipelineOptions pipelineOptions);

        IPipelineContext CreatePipelineContext();
    }
}
