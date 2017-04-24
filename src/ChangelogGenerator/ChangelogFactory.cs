using Semver;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public class ChangelogFactory : IChangelogFactory
    {
        /// <summary>
        /// Creates a <see cref="Changelog"/> instance. <see cref="Changelog.Versions"/>
        /// of returned instance are sorted by <see cref="Version.SemVersion"/> in descending order.
        /// </summary>
        /// <param name="pattern">
        /// Regex pattern that captures two groups per version. First group must be the semantic version 
        /// associated with the version. Second must be the notes associated with the version.
        /// </param>
        /// <param name="changelogText"></param>
        /// <returns>
        /// <see cref="Changelog"/>
        /// </returns>
        public IChangelog Build(string pattern, string changelogText)
        {
            MatchCollection matches = Regex.Matches(changelogText, pattern, RegexOptions.Singleline);
            SortedSet<IVersion> versions = new SortedSet<IVersion>();

            foreach (Match match in matches)
            {
                versions.Add(new Version
                {
                    Raw = match.Groups[0].Value,
                    SemVersion = SemVersion.Parse(match.Groups[1].Value),
                    Notes = match.Groups[2].Value.Trim()
                });
            }

            return new Changelog(versions);
        }
    }
}
