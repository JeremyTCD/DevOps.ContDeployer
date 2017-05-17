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
        public IEnumerable<IStep> CreatePipeline()
        {
            return new IStep[]{
                // TODO is there some way to verify that the correct options type is provided?
                new Step<ChangelogPlugin>(new ChangelogPluginOptions
                {
                        Branch = "master",
                        FileName = "changelog.md",
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
