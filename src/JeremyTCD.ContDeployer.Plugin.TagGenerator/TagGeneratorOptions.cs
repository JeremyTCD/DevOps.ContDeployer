using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    public class TagGeneratorOptions : IPluginOptions
    {
        public string TagName { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(TagName))
            {
                throw new Exception($"{nameof(TagGeneratorOptions)}: {nameof(TagName)} cannot be null or empty");
            }
        }
    }
}
