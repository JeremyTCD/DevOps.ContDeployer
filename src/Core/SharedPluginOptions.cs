namespace JeremyTCD.PipelinesCE.Core
{
    /// <summary>
    /// For backward and forward compatibility, all properties must have a default value.
    /// Properties must have nullable backing fields so there is some state that represents unset. 
    /// This facilitates overwriting of unset properties, <see cref="Combine(PipelinesCEOptions)"/>.
    /// </summary>
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
