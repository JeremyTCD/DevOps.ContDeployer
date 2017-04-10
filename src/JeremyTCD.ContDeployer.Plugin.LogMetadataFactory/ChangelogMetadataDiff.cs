using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDeployer
{
    public class ChangelogMetadataDiff
    {
        public List<Version> AddedVersions { get; set; } = new List<Version>();
        public List<Version> RemovedVersions { get; set; } = new List<Version>();
        public List<Version> ModifiedVersions { get; set; } = new List<Version>();
    }
}
