using Semver;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.LogMetadataFactory
{
    public class ChangelogMetadata
    {
        public List<Version> Versions { get; set; }

        public ChangelogMetadata()
        {
            Versions = new List<Version>();
        }

        // TODO pls test this
        // http://stackoverflow.com/questions/5958169/how-to-merge-two-sorted-arrays-into-a-sorted-array
        public ChangelogMetadataDiff Compare(ChangelogMetadata otherChangelogMetadata)
        {
            // ChangelogMetadata is in semantic version order by default
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
