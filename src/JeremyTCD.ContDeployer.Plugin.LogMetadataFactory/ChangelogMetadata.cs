using Semver;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDeployer
{
    public class ChangelogMetadata
    {
        /// <summary>
        /// List of Versions contained in a changelog
        /// </summary>
        public List<Version> Versions { get; }

        public ChangelogMetadata(List<Version> versions)
        {
            Versions = versions;
        }

        /// <summary>
        /// Creates a <see cref="ChangelogMetadataDiff"/> between this instance and another <see cref="ChangelogMetadata"/> instance.
        /// <see cref="ChangelogMetadata.Versions"/> of both instances must be in descending <see cref="Version.SemVersion"/> order.
        /// </summary>
        /// <param name="otherChangelogMetadata">
        /// If null, all versions in this instance are considered to be newly added.
        /// </param>
        /// <returns>
        /// <see cref="ChangelogMetadataDiff"/>
        /// </returns>
        public ChangelogMetadataDiff Diff(ChangelogMetadata otherChangelogMetadata)
        {
            ChangelogMetadataDiff result = new ChangelogMetadataDiff();

            if (otherChangelogMetadata == null)
            {
                foreach (Version version in Versions)
                {
                    result.AddedVersions.Add(version);
                }
            }
            else
            {
                List<Version> otherVersions = otherChangelogMetadata.Versions;
                int i = 0, j = 0;

                while (i < Versions.Count || j < otherChangelogMetadata.Versions.Count)
                {
                    Version thisVersion = i < Versions.Count ? Versions[i] : null;
                    Version otherVersion = j < otherVersions.Count ? otherVersions[j] : null;
                    int comparisonResult = thisVersion == null ? -1 :
                        otherVersion == null ? 1 :
                        thisVersion.SemVersion.CompareTo(otherVersion.SemVersion);
                    if (comparisonResult > 0)
                    {
                        result.AddedVersions.Add(thisVersion);
                        i++;
                    }
                    else if (comparisonResult < 0)
                    {
                        result.RemovedVersions.Add(otherVersion);
                        j++;
                    }
                    else
                    {
                        if (thisVersion.Raw != otherVersion.Raw)
                        {
                            result.ModifiedVersions.Add(thisVersion);
                        }

                        i++;
                        j++;
                    }
                }
            }

            return result;
        }
    }
}
