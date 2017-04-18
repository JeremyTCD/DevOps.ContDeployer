using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleasePublisher
{
    public class GithubReleasePublisherChangelogAdapter : IPlugin
    {
        /// <summary>
        /// Tags head
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object diffObject);
            Changelog diff = diffObject as Changelog;
            if (diff == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
            }

            foreach(ChangelogGenerator.Version version in diff.AddedVersions)
            {
                // Add GithubReleasePublisher to create release
            }

            // TODO version must include tag field
            //   - tag field must be used to create tag on head
            //      - dont forget to push to origin
            //   - tag must be provided as tag_name 
            foreach(ChangelogGenerator.Version version in diff.ModifiedVersions)
            {
                // Add GithubReleasePublisher to edit release
            }
        }
    }
}
