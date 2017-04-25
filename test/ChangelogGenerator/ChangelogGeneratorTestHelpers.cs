using NSubstitute;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator.Tests
{
    public class ChangelogGeneratorTestHelpers
    {
        public static IChangelogFactory CreateMockChangelogFactory(IChangelog changelog, string pattern, string text)
        {
            IChangelogFactory changelogFactory = Substitute.For<IChangelogFactory>();
            changelogFactory.Build(pattern, text).Returns(changelog);

            return changelogFactory;
        }

        public static IChangelog CreateMockChangelog(SortedSet<IVersion> versions = null)
        {
            IChangelog changelog = Substitute.For<IChangelog>();
            changelog.Versions.Returns(versions);
            return changelog;
        }
    }
}
