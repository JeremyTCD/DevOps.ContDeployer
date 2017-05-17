using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.Changelog.Tests.UnitTests
{
    public class ChangelogFactoryUnitTests
    {
        [Fact]
        public void Build_CreatesChangelogMetadata()
        {
            // Arrange
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

            ChangelogFactory factory = new ChangelogFactory();

            // Act
            IChangelog result = factory.Build(pattern, changelogText);

            // Assert
            Assert.NotNull(result.Versions);
            Assert.Equal(5, result.Versions.Count);
            List<IVersion> versions = result.Versions.ToList();
            Assert.Equal(version1, versions[0].SemVersion);
            Assert.Equal(notes1, versions[0].Notes);
            Assert.Equal(version2, versions[1].SemVersion);
            Assert.Equal(notes2, versions[1].Notes);
            Assert.Equal(version3, versions[2].SemVersion);
            Assert.Equal(notes3, versions[2].Notes);
            Assert.Equal(version4, versions[3].SemVersion);
            Assert.Equal(notes4, versions[3].Notes);
            Assert.Equal(version5, versions[4].SemVersion);
            Assert.Equal(notes5, versions[4].Notes);
        }
    }
}
