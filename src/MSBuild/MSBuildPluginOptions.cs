using JeremyTCD.PipelinesCE.Tools;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild
{
    /// <summary>
    /// Options are used as arguments for MSBuild.exe - https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-command-line-reference
    /// </summary>
    public class MSBuildPluginOptions : IPluginOptions
    {
        public string ProjOrSlnFile { get; set; } 
        public string Switches { get; set; } 

        public void Validate()
        {
            // Do nothing (Both properties are optional
        }
    }
}
