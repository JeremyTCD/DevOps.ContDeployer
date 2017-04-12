using Semver;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator
{
    public class Version : IComparable<Version>
    {
        public string Raw { get; set; }
        public SemVersion SemVersion { get; set; }
        public string Notes { get; set; }

        public int CompareTo(Version other)
        {
            return SemVersion.CompareTo(other.SemVersion);
        }
    }
}
