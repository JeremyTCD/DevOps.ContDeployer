using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer.Plugin.MSBuild
{
    public class MSBuildPluginOptions : IPluginOptions
    {
        public string ProjOrSlnFile { get; set; } = null;
        public string Switches { get; set; } = null;

        public void Validate()
        {
        }
    }
}
