namespace JeremyTCD.PipelinesCE.PluginAndConfigTools
{
    public interface IPipelineContextFactory
    {
        IPipelineContextFactory AddPipelineOptions(PipelineOptions pipelineOptions);

        IPipelineContext CreatePipelineContext();
    }
}
