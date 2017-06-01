namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class PipelineOptions
    {
        private bool? _verbose;
        private bool? _dryRun;

        public virtual bool Verbose
        {
            get
            {
                return _verbose ?? false;
            }
            set
            {
                _verbose = value;
            }
        }
        public virtual bool DryRun
        {
            get
            {
                return _dryRun ?? false;
            }
            set
            {
                _dryRun = value;
            }
        }
        public virtual string Project { get; set; }
        public virtual string Pipeline { get; set; }

        /// <summary>
        /// Overwrites each property that has not been set with the value of its equivalent property in <paramref name="secondary"/>
        /// </summary>
        /// <param name="secondary"></param>
        public virtual PipelineOptions Combine(PipelineOptions secondary)
        {
            if (_verbose == null)
            {
                Verbose = secondary.Verbose;
            }

            if (_dryRun == null)
            {
                DryRun = secondary.DryRun;
            }

            if (Project == null)
            {
                Project = secondary.Project;
            }

            if (Pipeline == null)
            {
                Pipeline = secondary.Pipeline;
            }

            return this;
        }
    }
}
