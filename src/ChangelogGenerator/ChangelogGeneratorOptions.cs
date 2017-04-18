using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public class ChangelogGeneratorOptions : IPluginOptions
    {
        public string FileName { get; set; } = "changelog.md";
        public string Branch { get; set; } = "master";
        // http://semver.org/
        public string Pattern { get; set; } = @"##[ \t]+(\d*\.\d*\.\d*(?:-[a-zA-Z0-9\.-]+)?(?:\+[a-zA-Z0-9\.-]+)?)(.*?)(?=##|$)";

        public void Validate()
        {
            // Do nothing
        }
    }
}