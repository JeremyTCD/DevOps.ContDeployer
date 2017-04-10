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
        /// <param name="otherChangelogMetadata"></param>
        /// <returns></returns>
        public ChangelogMetadataDiff Diff(ChangelogMetadata otherChangelogMetadata)
        {
            // TODO: if other is null, add all versions to newly added 

            ChangelogMetadataDiff result = new ChangelogMetadataDiff();
            int i = 0, j = 0;

            while(i < Versions.Count && j < otherChangelogMetadata.Versions.Count)
            {
                Version thisVersion = Versions[i];
                Version otherVersion = otherChangelogMetadata.Versions[j];
                int comparisonResult = thisVersion.SemVersion.CompareTo(otherVersion);
                if ( comparisonResult > 1)
                {
                    result.AddedVersions.Add(thisVersion);
                    i++;
                }
                else if(comparisonResult < 1)
                {
                    result.RemovedVersions.Add(otherVersion);
                    j++;
                }
                else
                {
                    if(thisVersion.Raw != otherVersion.Raw)
                    {
                        result.ModifiedVersions.Add(thisVersion);
                    }

                    i++;
                    j++;
                }
            }

            return result;
        }
    }
}
