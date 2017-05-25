namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class PipelineOptions
    {
        public virtual string PipelineName { get; set; }
        public virtual bool DryRun { get; set; } = false;
    }
}
