using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleasePublisher
{
    public class GithubReleasePublisherChangelogAdapterOptions : IPluginOptions
    {
        public string Token { get; set; }
        public string Owner { get; set; }
        public string Repository { get; set; }

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
