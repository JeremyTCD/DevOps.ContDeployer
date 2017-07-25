namespace JeremyTCD.PipelinesCE.Core
{
    public class SharedPluginOptions
    {
        public const bool DefaultDryRun = false;
        
        private bool? _dryRun;

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

        public virtual SharedPluginOptions Combine(SharedPluginOptions secondary)
        {
            if (_dryRun == null)
            {
                DryRun = secondary.DryRun;
            }

            return this;
        }
    }
}
