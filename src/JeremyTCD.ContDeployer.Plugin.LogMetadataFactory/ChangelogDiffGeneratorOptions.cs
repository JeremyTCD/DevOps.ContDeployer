using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator
{
    public class ChangelogDiffGeneratorOptions : IPluginOptions
    {
        public string FileName { get; set; } = "changelog.md";
        public string Branch { get; set; } = "master";
        public string Pattern { get; set; } = @"##[ \t]+(\d*\.\d*\.\d*)(.*?)(?=##[ \t]+\d*\.\d*\.\d*|$)";

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}