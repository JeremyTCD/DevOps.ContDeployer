using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer.Plugin.GitTags
{
    public class GitTagsChangelogAdapter : IPlugin
    {
        /// <summary>
        /// Compares <see cref="Changelog"/> and git tags. If latest version has no corresponding tag (new version), 
        /// adds <see cref="GitTags"/> step to tag head.
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="PipelineContext.SharedData"/> does not contain <see cref="Changelog"/> instance
        /// </exception>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            Changelog changelog = changelogObject as Changelog;
            if (changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
            }

            List<ChangelogGenerator.Version> versions = changelog.Versions.ToList();

            bool tagsConsistentWithChangelog = true;

            for (int i = 0; i < versions.Count; i++)
            {
                ChangelogGenerator.Version version = versions[i];

                if (pipelineContext.Repository.Tags[version.SemVersion.ToString()] == null)
                {
                    tagsConsistentWithChangelog = false;

                    if (i == 0)
                    {
                        GitTagsOptions gitTagsOptions = new GitTagsOptions
                        {
                            TagName = version.SemVersion.ToString()
                        };
                        Step gitTagsStep = new Step(nameof(GitTags), gitTagsOptions);
                        pipelineContext.Steps.AddFirst(gitTagsStep);

                        stepContext.Logger.LogInformation($"New version \"{version.SemVersion.ToString()}\"" +
                            $"has no corresponding tag, added {nameof(GitTags)} step");
                    }
                    else
                    {
                        // TODO Each version should point to a commit, corresponding commits should be
                        // tagged
                        stepContext.Logger.LogWarning($"Version \"{version.SemVersion.ToString()}\" has no corresponding tag");
                    }
                }
            }

            if (tagsConsistentWithChangelog)
            {
                stepContext.
                    Logger.
                    LogInformation("Tags consistent with changelog");
            }
        }
    }
}
