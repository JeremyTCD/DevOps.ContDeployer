using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public class Changelog
    {
        /// <summary>
        /// Set of versions ordered in descending semver order (latest first)
        /// </summary>
        public SortedSet<Version> Versions { get; }

        public Changelog(SortedSet<Version> versions)
        {
            Versions = versions;
        }
    }
}
