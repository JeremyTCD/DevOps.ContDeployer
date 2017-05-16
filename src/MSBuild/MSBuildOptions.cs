using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer.Plugin.MSBuild
{
    public class MSBuildOptions : IPluginOptions
    {
        public string ProjOrSlnFile { get; set; } = null;
        public string Switches { get; set; } = null;

        public void Validate()
        {
        }
    }
}
