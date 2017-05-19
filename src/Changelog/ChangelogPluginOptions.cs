using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer.Plugin.Changelog
{
    public class ChangelogPluginOptions : IPluginOptions
    {
        public virtual string File { get; set; } = "changelog.md";

        /// <summary>
        /// Branch to checkout. Defaults to master if unspecified.
        /// </summary>
        public virtual string Branch { get; set; } = "master";
        // http://semver.org/
        public virtual string Pattern { get; set; } = @"##[ \t]+(\d*\.\d*\.\d*(?:-[a-zA-Z0-9\.-]+)?(?:\+[a-zA-Z0-9\.-]+)?)(.*?)(?=##|$)";

        public virtual void Validate()
        {
            // Do nothing
        }
    }
}