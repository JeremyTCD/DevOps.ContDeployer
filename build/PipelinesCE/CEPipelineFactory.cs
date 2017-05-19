using JeremyTCD.ContDeployer;
using JeremyTCD.ContDeployer.Plugin.Changelog;
using JeremyTCD.ContDeployer.Plugin.Git;
using JeremyTCD.ContDeployer.Plugin.MSBuild;
using JeremyTCD.ContDeployer.PluginTools;
using System.Collections.Generic;

namespace PipelinesCE
{
    public class CEPipelineFactory : IPipelineFactory
    {
        public IEnumerable<IStep> Build()
        {
            return new IStep[]{
                new Step<ChangelogPlugin>(new ChangelogPluginOptions
                {
                        Branch = "master",
                        File = "changelog.md",
                        Pattern = ""
                }),
                new Step<MSBuildPlugin>(new MSBuildPluginOptions
                {
                    
                }),
                new Step<GitPlugin>(new GitPluginOptions
                {

                })
            };
        }
    }
}
