using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.Plugin.GitTags
{
    public class GitTagsOptions : IPluginOptions
    {
        public string TagName { get; set; } = null;

        public void Validate()
        {
            if (string.IsNullOrEmpty(TagName))
            {
                throw new Exception($"{nameof(GitTagsOptions)}: {nameof(TagName)} cannot be null or empty");
            }
        }
    }
}
