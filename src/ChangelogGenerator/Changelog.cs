using Semver;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public class Changelog
    {
        /// <summary>
        /// List of Versions contained in a changelog
        /// </summary>
        public List<Version> Versions { get; }

        public Changelog(List<Version> versions)
        {
            Versions = versions;
        }
    }
}
