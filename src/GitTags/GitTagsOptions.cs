using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    public class TagGeneratorOptions : IPluginOptions
    {
        public string TagName { get; set; } = null;

        public void Validate()
        {
            if (string.IsNullOrEmpty(TagName))
            {
                throw new Exception($"{nameof(TagGeneratorOptions)}: {nameof(TagName)} cannot be null or empty");
            }
        }
    }
}
