using Semver;
using System;

namespace JeremyTCD.PipelinesCE.Plugin.Changelog
{
    public interface IVersion : IComparable<IVersion>
    {
        string Raw { get; set; }
        SemVersion SemVersion { get; set; }
        string Notes { get; set; }
    }
}
