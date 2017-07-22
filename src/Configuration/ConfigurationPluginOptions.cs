using System;
using JeremyTCD.PipelinesCE.Tools;

namespace JeremyTCD.PipelinesCE.Plugin.Configuration
{
    public class ConfigurationPluginOptions : IPluginOptions
    {
        public virtual PipelineOptions PipelineOptions {get;set;}

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
