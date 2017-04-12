using Semver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator
{
    public class ChangelogMetadataFactory
    {
        /// <summary>
        /// Creates a <see cref="ChangelogMetadata"/> instance. <see cref="ChangelogMetadata.Versions"/>
        /// of returned instance are sorted by <see cref="Version.SemVersion"/> in descending order.
        /// </summary>
        /// <param name="pattern">
        /// Regex pattern that captures two groups per version. First group must be the semantic version 
        /// associated with the version. Second must be the notes associated with the version.
        /// </param>
        /// <param name="changelogText"></param>
        /// <returns>
        /// <see cref="ChangelogMetadata"/>
        /// </returns>
        public ChangelogMetadata Build(string pattern, string changelogText)
        {
            MatchCollection matches = Regex.Matches(changelogText, pattern, RegexOptions.Singleline);
            List<Version> versions = new List<Version>();

            foreach (Match match in matches)
            {
                versions.Add(new Version()
                {
                    Raw = match.Groups[0].Value,
                    SemVersion = SemVersion.Parse(match.Groups[1].Value),
                    Notes = match.Groups[2].Value.Trim()
                });
            }

            versions.Sort();
            // Descending order
            versions.Reverse();

            return new ChangelogMetadata(versions);
        }
    }
}
