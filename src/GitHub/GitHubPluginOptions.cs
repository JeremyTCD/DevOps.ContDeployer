using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Octokit;
using System;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Plugin.GitHub
{
    public class GitHubPluginOptions : IPluginOptions
    {
        public virtual string Token { get; set; }
        public virtual string Owner { get; set; }
        public virtual string Repository { get; set; }
        public virtual IList<NewRelease> NewReleases { get; set; } = new List<NewRelease>();
        public virtual IList<ModifiedRelease> ModifiedReleases { get; set; } = new List<ModifiedRelease>();

        public virtual void Validate()
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
