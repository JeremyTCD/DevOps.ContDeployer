using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.Changelog
{
    public interface IChangelog
    {
        SortedSet<IVersion> Versions { get; }
    }
}
