using JeremyTCD.PipelinesCE.PluginTools;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild
{
    /// <summary>
    /// Wraps MSBuild.exe - https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-command-line-reference.
    /// Eventually, should utilize Microsoft.Build libraries directly or Microsoft.CodeAnalysis (Roslyn). Utilizing
    /// libraries eliminates the need to download msbuild separately on each build or to configure a ci server to have
    /// it installed. At the time of writing, Microsoft.Build libraries do not support resolving of custom SDKs 
    /// (properties, tasks and targets) and Roslyn is not fully cross platform (MSBuildWorkspace). 
    /// </summary>
    public class MSBuildClient : IMSBuildClient
    {
        private IProcessManager _processManager { get; }

        public MSBuildClient(IProcessManager processManager)
        {
            _processManager = processManager;
        }

        /// <summary>
        /// Build a project or a solution
        /// </summary>
        /// <param name="projOrSlnFile">
        /// Path of a *proj or sln file. If null, executes msbuild.exe in current directory
        /// </param>
        /// <param name="switches">
        /// Ignored if null. https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-command-line-reference
        /// </param>
        public void Build(string projOrSlnFile = null, string switches = null)
        {
            _processManager.Execute("msbuild.exe", 
                string.Concat(switches ?? "", " ", projOrSlnFile ?? ""));
        }
    } 
}
