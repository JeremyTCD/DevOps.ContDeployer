namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class PipelineOptions
    { 
        public virtual bool Verbose { get; set; }
        public virtual string Project { get; set; } = "PipelinesCE.csproj";
        public virtual string Pipeline { get; set; }
        public virtual bool DryRun { get; set; }
    }
}
