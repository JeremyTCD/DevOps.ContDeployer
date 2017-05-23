using JeremyTCD.PipelinesCE.PluginTools;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild
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
