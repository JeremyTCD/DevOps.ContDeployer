using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public interface IChangelog
    {
        SortedSet<IVersion> Versions { get; }
    }
}
