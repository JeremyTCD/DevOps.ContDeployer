namespace JeremyTCD.PipelinesCE.Core
{
    public class PipelinesCEOptions
    {
        public const string DefaultLogFileName = "PipelinesCE.log";
        public const string DefaultProjectFileName = "PipelinesCEConfig.csproj";
        public const string DefaultPipeline = "Default";
        public const bool DefaultVerbose = false;
        public const bool DefaultDebug = false;

        public virtual string LogFile { get; set; } // Default only known at runtime
        public virtual string ProjectFile { get; set; } // Default only known at runtime

        public virtual bool Debug { get; set; } = DefaultDebug;
        public virtual bool Verbose { get; set; } = DefaultVerbose;
        public virtual string Pipeline { get; set; } = DefaultPipeline;
    }
}
