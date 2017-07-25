using System;
using JeremyTCD.PipelinesCE.Core;

namespace JeremyTCD.PipelinesCE.Plugin.Configuration
{
    public class ConfigurationPluginOptions : IPluginOptions
    {
        public virtual PipelinesCEOptions PipelineOptions {get;set;}

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
