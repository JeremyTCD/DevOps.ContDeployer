using Semver;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator.Tests.UnitTests
{
    public class ChangelogMetadataTests
    {
        [Fact]
        public void Diff_AccountsForAddedVersions()
        {
            // Arrange
            ChangelogMetadata oldMetadata = new ChangelogMetadata(
                new List<Version>
                {
                    new Version(){SemVersion = SemVersion.Parse("1.1.0")},
                    new Version(){SemVersion = SemVersion.Parse("1.0.0")}
                });

            ChangelogMetadata newMetadata = new ChangelogMetadata(
                new List<Version>
                {
                    new Version(){SemVersion = SemVersion.Parse("2.1.1")}, // Add Version to start of list
                    new Version(){SemVersion = SemVersion.Parse("1.1.0")},
                    new Version(){SemVersion = SemVersion.Parse("1.0.1")}, // Add Version to middle of list
                    new Version(){SemVersion = SemVersion.Parse("1.0.0")},
                    new Version(){SemVersion = SemVersion.Parse("1.0.0-alpha")} // Add Version to end of list
                });

            // Act
            ChangelogDiff result = newMetadata.Diff(oldMetadata);

            // Assert 
            Assert.NotNull(result.AddedVersions);
            Assert.Equal(3, result.AddedVersions.Count);
            Assert.Equal("2.1.1", result.AddedVersions[0].SemVersion.ToString());
            Assert.Equal("1.0.1", result.AddedVersions[1].SemVersion.ToString());
            Assert.Equal("1.0.0-alpha", result.AddedVersions[2].SemVersion.ToString());
        }

        [Fact]
        public void Diff_AccountsForRemovedVersions()
        {
            // Arrange
            ChangelogMetadata oldMetadata = new ChangelogMetadata(
                new List<Version>
                {
                    new Version(){SemVersion = SemVersion.Parse("2.1.1")}, // Remove Version from start of list
                    new Version(){SemVersion = SemVersion.Parse("1.1.0")},
                    new Version(){SemVersion = SemVersion.Parse("1.0.1")}, // Remove Version from middle of list
                    new Version(){SemVersion = SemVersion.Parse("1.0.0")},
                    new Version(){SemVersion = SemVersion.Parse("1.0.0-alpha")} // Remove Version from end of list
                });

            ChangelogMetadata newMetadata = new ChangelogMetadata(
                new List<Version>
                {
                    new Version(){SemVersion = SemVersion.Parse("1.1.0")},
                    new Version(){SemVersion = SemVersion.Parse("1.0.0")}
                });

            // Act
            ChangelogDiff result = newMetadata.Diff(oldMetadata);

            // Assert 
            Assert.NotNull(result.RemovedVersions);
            Assert.Equal(3, result.RemovedVersions.Count);
            Assert.Equal("2.1.1", result.RemovedVersions[0].SemVersion.ToString());
            Assert.Equal("1.0.1", result.RemovedVersions[1].SemVersion.ToString());
            Assert.Equal("1.0.0-alpha", result.RemovedVersions[2].SemVersion.ToString());
        }

        [Fact]
        public void Diff_AccountsForModifiedVersions()
        {
            // Arrange
            ChangelogMetadata oldMetadata = new ChangelogMetadata(
                new List<Version>
                {
                    new Version(){SemVersion = SemVersion.Parse("1.1.1"), Raw = "## 1.1.1\nOld"},
                    new Version(){SemVersion = SemVersion.Parse("1.1.0"), Raw = "## 1.1.0\nOld"},
                    new Version(){SemVersion = SemVersion.Parse("1.0.0"), Raw = "## 1.0.0\nOld"}
                });

            ChangelogMetadata newMetadata = new ChangelogMetadata(
                new List<Version>
                {
                    new Version(){SemVersion = SemVersion.Parse("1.1.1"), Raw = "## 1.1.1\nNew"},
                    new Version(){SemVersion = SemVersion.Parse("1.1.0"), Raw = "## 1.1.0\nNew"},
                    new Version(){SemVersion = SemVersion.Parse("1.0.0"), Raw = "## 1.0.0\nNew"}
                });

            // Act
            ChangelogDiff result = newMetadata.Diff(oldMetadata);

            // Assert 
            Assert.NotNull(result.ModifiedVersions);
            Assert.Equal(3, result.ModifiedVersions.Count);
            Assert.Equal("## 1.1.1\nNew", result.ModifiedVersions[0].Raw);
            Assert.Equal("## 1.1.0\nNew", result.ModifiedVersions[1].Raw);
            Assert.Equal("## 1.0.0\nNew", result.ModifiedVersions[2].Raw);
        }

        [Fact]
        public void Diff_ConsidersAllVersionsAsAddedIfOtherIsNull()
        {
            // Arrange
            ChangelogMetadata newMetadata = new ChangelogMetadata(
                new List<Version>
                {
                    new Version(){SemVersion = SemVersion.Parse("1.1.1")},
                    new Version(){SemVersion = SemVersion.Parse("1.1.0")},
                    new Version(){SemVersion = SemVersion.Parse("1.0.0")}
                });

            // Act
            ChangelogDiff result = newMetadata.Diff(null);

            // Assert 
            Assert.NotNull(result.AddedVersions);
            Assert.Equal(3, result.AddedVersions.Count);
            Assert.Equal("1.1.1", result.AddedVersions[0].SemVersion.ToString());
            Assert.Equal("1.1.0", result.AddedVersions[1].SemVersion.ToString());
            Assert.Equal("1.0.0", result.AddedVersions[2].SemVersion.ToString());
        }
    }
}
