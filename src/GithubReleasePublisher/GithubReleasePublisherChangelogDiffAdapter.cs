using JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using System;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleasePublisher
{
    public class GithubReleasePublisherChangelogDiffAdapter : IPlugin
    {
        /// <summary>
        /// Tags head
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            pipelineContext.SharedData.TryGetValue(nameof(ChangelogDiff), out object diffObject);
            ChangelogDiff diff = diffObject as ChangelogDiff;
            if (diff == null)
            {
                throw new InvalidOperationException($"No {nameof(ChangelogDiff)} in {nameof(pipelineContext.SharedData)}");
            }

            foreach(ChangelogDiffGenerator.Version version in diff.AddedVersions)
            {
                // Add GithubReleasePublisher to create release
            }

            // TODO version must include tag field
            //   - tag field must be used to create tag on head
            //      - dont forget to push to origin
            //   - tag must be provided as tag_name 
            foreach(ChangelogDiffGenerator.Version version in diff.ModifiedVersions)
            {
                // Add GithubReleasePublisher to edit release
            }
        }
    }
}
