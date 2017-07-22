using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JeremyTCD.PipelinesCE.Tools
{
    /// <summary>
    /// For backward and forward compatibility, all properties must have a default value.
    /// Properties must have nullable backing fields so there is some state that represents unset. 
    /// This facilitates overwriting of unset properties, <see cref="Combine(PipelineOptions)"/>.
    /// </summary>
    public class PipelineOptions
    {
        public const string LogFileFormat = nameof(PipelinesCE) + "-{Date}.log";
        public const LogLevel VerboseMinLogLevel = LogLevel.Debug;
        public const LogLevel DefaultMinLogLevel = LogLevel.Information;
        public const string DefaultPipeline = "Default";
        public const string DefaultProject = "PipelinesCEConfig.csproj";
        public const bool DefaultDryRun = false;
        public const bool DefaultVerbose = false;
        public const string EntryAssemblyName = "JeremyTCD.PipelinesCE.ConfigHost";
        public const string EntryClassName = "JeremyTCD.PipelinesCE.ConfigHost.ConfigHostStartup";

        private bool? _verbose;
        private bool? _dryRun;
        private string _project;
        private string _pipeline;



        /// <summary>
        /// Defaults to false
        /// </summary>
        public virtual bool Verbose
        {
            get
            {
                return _verbose ?? DefaultVerbose;
            }
            set
            {
                _verbose = value;
            }
        }

        /// <summary>
        /// Defaults to false
        /// </summary>
        public virtual bool DryRun
        {
            get
            {
                return _dryRun ?? DefaultDryRun;
            }
            set
            {
                _dryRun = value;
            }
        }

        /// <summary>
        /// Defaults to PipelinesCE.csproj
        /// </summary>
        public virtual string Project
        {
            get
            {
                return _project ?? DefaultProject;
            }
            set
            {
                _project = value;
            }
        }

        /// <summary>
        /// If there is only 1 <see cref="IPipelineFactory"/>, defaults to the sole <see cref="Pipeline"/>
        /// </summary>
        public virtual string Pipeline
        {
            get
            {
                return _pipeline ?? DefaultPipeline;
            }
            set
            {
                _pipeline = value;
            }
        }


        /// <summary>
        /// Overwrites each property that has not been set with the value of its equivalent property in <paramref name="secondary"/>
        /// </summary>
        /// <param name="secondary"></param>
        public virtual PipelineOptions Combine(PipelineOptions secondary)
        {
            if (_dryRun == null)
            {
                DryRun = secondary.DryRun;
            }

            if (_project == null)
            {
                Project = secondary.Project;
            }

            if (_pipeline == null)
            {
                Pipeline = secondary.Pipeline;
            }

            return this;
        }
    }
}
