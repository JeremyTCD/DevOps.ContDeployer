using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Plugin.Changelog
{
    public interface IChangelog
    {
        SortedSet<IVersion> Versions { get; }
    }
}
