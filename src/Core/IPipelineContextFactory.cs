namespace JeremyTCD.PipelinesCE.Core
{
    public interface IPipelineContextFactory
    {
        IPipelineContextFactory AddSharedPluginOptions(SharedPluginOptions sharedPluginOptions);

        IPipelineContext CreatePipelineContext();
    }
}
