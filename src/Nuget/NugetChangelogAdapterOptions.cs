using JeremyTCD.PipelinesCE.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.PipelinesCE.Plugin.Nuget
{
    public class NugetChangelogAdapterOptions : IPluginOptions
    {
        public virtual string Source { get; set; } 
        public virtual string PackageName { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(PackageName))
            {
                throw new Exception($"{nameof(PackageName)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(Source))
            {
                throw new Exception($"{nameof(Source)} cannot be null or empty");
            }
            // TODO
        }
    }
}
