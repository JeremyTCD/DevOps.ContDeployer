using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.Plugin.GitHub
{
    public class GitHubChangelogAdapterOptions : IPluginOptions
    {
        public virtual string Token { get; set; }
        public virtual string Owner { get; set; }
        public virtual string Repository { get; set; }
        // Optional since only necessary if tag that release points to does not already exist
        public virtual string Commitish { get; set; } = "";

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
