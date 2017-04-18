using Semver;
using System;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
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
