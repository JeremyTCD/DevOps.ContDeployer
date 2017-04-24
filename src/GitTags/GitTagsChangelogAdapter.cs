using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer.Plugin.GitTags
{
    public class GitTagsChangelogAdapter : PluginBase
    {
        private IChangelog _changelog { get; }

        /// <summary>
        /// Creates a <see cref="GitTagsChangelogAdapter"/> instance
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="IPipelineContext.SharedData"/> does not contain <see cref="IChangelog"/> instance
        /// </exception>
        public GitTagsChangelogAdapter(IPipelineContext pipelineContext, IStepContext stepContext) : 
            base(pipelineContext, stepContext)
        {
            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            _changelog = changelogObject as IChangelog;
            if (_changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
            }
        }

        /// <summary>
        /// Compares <see cref="Changelog"/> and git tags. If latest version has no corresponding tag (new version), 
        /// adds <see cref="GitTags"/> step to tag head.
        /// </summary>
        public override void Run()
        {
            List<IVersion> versions = _changelog.Versions.ToList();

            bool tagsConsistentWithChangelog = true;

            for (int i = 0; i < versions.Count; i++)
            {
                IVersion version = versions[i];

                if (PipelineContext.Repository.Tags[version.SemVersion.ToString()] == null)
                {
                    tagsConsistentWithChangelog = false;

                    if (i == 0)
                    {
                        GitTagsOptions gitTagsOptions = new GitTagsOptions
                        {
                            TagName = version.SemVersion.ToString()
                        };
                        IStep gitTagsStep = PipelineContext.StepFactory.Build(nameof(GitTags), gitTagsOptions);
                        PipelineContext.Steps.AddFirst(gitTagsStep);

                        StepContext.Logger.LogInformation($"New version \"{version.SemVersion.ToString()}\"" +
                            $"has no corresponding tag, added {nameof(GitTags)} step");
                    }
                    else
                    {
                        // TODO Each version should point to a commit, corresponding commits should be
                        // tagged
                        StepContext.Logger.LogWarning($"Version \"{version.SemVersion.ToString()}\" has no corresponding tag");
                    }
                }
            }

            if (tagsConsistentWithChangelog)
            {
                StepContext.
                    Logger.
                    LogInformation("Tags consistent with changelog");
            }
        }
    }
}
