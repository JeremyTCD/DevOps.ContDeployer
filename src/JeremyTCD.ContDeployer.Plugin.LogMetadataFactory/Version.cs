using Semver;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.LogMetadataFactory
{
    public class Version
    {
        public string Raw { get; set; }
        public SemVersion SemVersion { get; set; }
        public string Notes { get; set; }
    }
}
