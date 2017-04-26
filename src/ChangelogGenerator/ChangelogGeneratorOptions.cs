using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public class ChangelogGeneratorOptions : IPluginOptions
    {
        public virtual string FileName { get; set; } = "changelog.md";
        public virtual string Branch { get; set; } = "master";
        // http://semver.org/
        public virtual string Pattern { get; set; } = @"##[ \t]+(\d*\.\d*\.\d*(?:-[a-zA-Z0-9\.-]+)?(?:\+[a-zA-Z0-9\.-]+)?)(.*?)(?=##|$)";

        public virtual void Validate()
        {
            // Do nothing
        }
    }
}