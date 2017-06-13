﻿namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class PipelineOptions
    {
        public const string DefaultPipeline = "Default";
        public const string DefaultProject = "PipelinesCE.csproj";
        public const bool DefaultDryRun = false;
        public const bool DefaultVerbose = false;

        private bool? _dryRun;
        private bool? _verbose;
        private string _project;
        private string _pipeline;

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

        public bool Verbose
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
