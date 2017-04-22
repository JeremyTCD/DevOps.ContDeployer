using System;
using JeremyTCD.ContDeployer.PluginTools;
using Octokit;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.GitHubReleases
{
    public class GitHubReleasesOptions : IPluginOptions
    {
        public string Token { get; set; }
        public string Owner { get; set; }
        public string Repository { get; set; }
        public List<NewRelease> NewReleases { get; set; } = new List<NewRelease>();
        public List<ReleaseUpdate> ReleaseUpdates { get; set; } = new List<ReleaseUpdate>();

        public void Validate()
        {
            if (string.IsNullOrEmpty(Token))
            {
                throw new Exception($"{nameof(Token)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(Owner))
            {
                throw new Exception($"{nameof(Owner)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(Repository))
            {
                throw new Exception($"{nameof(Repository)} cannot be null or empty");
            }
        }
    }
}
