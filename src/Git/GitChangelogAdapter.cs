using JeremyTCD.PipelinesCE.Plugin.Changelog;
using JeremyTCD.PipelinesCE.Tools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JeremyTCD.PipelinesCE.Plugin.Git
{
    public class GitChangelogAdapter : IPlugin
    {
        private IRepository _repository { get; }

        /// <summary>
        /// Creates a <see cref="GitChangelogAdapter"/> instance
        /// </summary>
        /// <param name="repositoryFactory"></param>
        public GitChangelogAdapter(IRepositoryFactory repositoryFactory)
        {
            _repository = repositoryFactory.Build(Directory.GetCurrentDirectory());
        }

        /// <summary>
        /// Compares <see cref="Changelog"/> and git tags. If latest version has no corresponding tag (new version), 
        /// adds <see cref="GitPlugin"/> step to tag head.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="IPipelineContext.SharedData"/> does not contain <see cref="IChangelog"/> instance
        /// </exception>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            IChangelog changelog = changelogObject as IChangelog;
            if (changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
            }

            List<IVersion> versions = changelog.Versions.ToList();
            bool tagsConsistentWithChangelog = true;

            for (int i = 0; i < versions.Count; i++)
            {
                IVersion version = versions[i];

                if (_repository.Tags[version.SemVersion.ToString()] == null)
                {
                    tagsConsistentWithChangelog = false;

                    if (i == 0)
                    {
                        IStep gitPluginStep = new Step<GitPlugin>(new GitPluginOptions
                        {
                            TagName = version.SemVersion.ToString()
                        });
                        stepContext.RemainingSteps.AddFirst(gitPluginStep);

                        stepContext.Logger.LogInformation($"New version \"{version.SemVersion.ToString()}\"" +
                            $"has no corresponding tag, added {nameof(GitPlugin)} step");
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
