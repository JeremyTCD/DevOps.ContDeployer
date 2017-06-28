using JeremyTCD.PipelinesCE;
using JeremyTCD.PipelinesCE.Plugin.Changelog;
using JeremyTCD.PipelinesCE.Plugin.Configuration;
using JeremyTCD.PipelinesCE.Plugin.Git;
using JeremyTCD.PipelinesCE.Plugin.MSBuild;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;

namespace PipelinesCE
{
    public class CEPipelineFactory : IPipelineFactory
    {
        public Pipeline CreatePipeline()
        {
            return new Pipeline(
                new IStep[]{
                    new Step<ConfigurationPlugin>(new ConfigurationPluginOptions
                    {
                        PipelineOptions = new PipelineOptions
                        {
                            DryRun = false
                        }
                    }),
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
                }
            );
        }
    }
}
