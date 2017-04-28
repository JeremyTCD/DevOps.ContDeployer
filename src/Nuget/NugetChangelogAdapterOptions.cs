using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.Nuget
{
    public class NugetChangelogAdapterOptions : IPluginOptions
    {
        public virtual IList<string> Sources { get; set; } = new List<string>();
        public virtual string PackageName { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(PackageName))
            {
                throw new Exception($"{nameof(PackageName)} cannot be null or empty");
            }

            if(Sources.Count == 0)
            {
                throw new Exception($"{nameof(Sources)} cannot be empty");
            }
            // TODO
        }
    }
}
