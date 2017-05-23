using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Plugin.Changelog
{
    public class Changelog : IChangelog
    {
        /// <summary>
        /// Set of versions ordered in descending semver order (latest first)
        /// </summary>
        public SortedSet<IVersion> Versions { get; }

        public Changelog(SortedSet<IVersion> versions)
        {
            Versions = versions;
        }
    }
}
