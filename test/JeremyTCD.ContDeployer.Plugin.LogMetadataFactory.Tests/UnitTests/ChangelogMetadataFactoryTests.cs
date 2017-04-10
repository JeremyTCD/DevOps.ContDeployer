using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDeployer.Tests.UnitTests
{
    public class ChangelogMetadataFactoryTests
    {
        [Fact]
        public void Build_CreatesChangelogMetadata()
        {
            // Arrange
            // http://semver.org/
            string pattern = @"##[ \t]+(\d*\.\d*\.\d*(?:-[a-zA-Z0-9\.-]+)?(?:\+[a-zA-Z0-9\.-]+)?)(.*?)(?=##|$)";
            string version1 = "2.1.1";
            string notes1 = "Body211";
            string version2 = "1.1.1";
            string notes2 = "Body111";
            string version3 = "1.0.1";
            string notes3 = "Body101";
            string version4 = "1.0.0";
            string notes4 = "Body100";
            string version5 = "0.1.0-beta";
            string notes5 = "Body010beta";

            string changelogText = $"## {version1}\n{notes1}\n" +
                $"## {version3}\n{notes3}\n" + 
                $"## {version2}\n{notes2}\n" + 
                $"## {version4}\n{notes4}\n" +
                $"## {version5}\n{notes5}\n";

            ChangelogMetadataFactory factory = new ChangelogMetadataFactory();

            // Act
            ChangelogMetadata result = factory.Build(pattern, changelogText);

            // Assert
            Assert.NotNull(result.Versions);
            Assert.Equal(5, result.Versions.Count);
            Assert.Equal(version1, result.Versions[0].SemVersion);
            Assert.Equal(notes1, result.Versions[0].Notes);
            Assert.Equal(version2, result.Versions[1].SemVersion);
            Assert.Equal(notes2, result.Versions[1].Notes);
            Assert.Equal(version3, result.Versions[2].SemVersion);
            Assert.Equal(notes3, result.Versions[2].Notes);
            Assert.Equal(version4, result.Versions[3].SemVersion);
            Assert.Equal(notes4, result.Versions[3].Notes);
            Assert.Equal(version5, result.Versions[4].SemVersion);
            Assert.Equal(notes5, result.Versions[4].Notes);
        }
    }
}
