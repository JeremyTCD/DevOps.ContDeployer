using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using LibGit2Sharp.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public class ChangelogGenerator : PluginBase
    {
        private ChangelogGeneratorOptions _options { get; set; }
        private IChangelogFactory _changelogFactory { get; set; }

        /// <summary>
        /// Creates a <see cref="ChangelogGenerator"/> instance
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IStepContext.Options"/> is null
        /// </exception>
        public ChangelogGenerator(IPipelineContext pipelineContext, IStepContext stepContext, IChangelogFactory changelogFactory) : 
            base(pipelineContext, stepContext)
        {
            _options = stepContext.Options as ChangelogGeneratorOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(ChangelogGeneratorOptions)} required");
            }

            _changelogFactory = changelogFactory;
        }

        /// <summary>
        /// Generates <see cref="Changelog"/> and inserts it into <see cref="IPipelineContext.SharedData"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="_options.Filename"/> is an empty 
        /// </exception>
        public override void Run()
        {
            // TODO checkout branch specified in options
            // Get changelog text
            string changelogText = GetChangelogText();
            if (string.IsNullOrEmpty(changelogText))
            {
                throw new InvalidOperationException($"File with name \"{_options.FileName}\" is empty");
            }

            // Build changelog
            IChangelog changelog = _changelogFactory.Build(_options.Pattern, changelogText);

            PipelineContext.SharedData[nameof(Changelog)] = changelog;
            StepContext.Logger.LogInformation($"{nameof(Changelog)} generated");
        }

        /// <summary>
        /// Gets contents of <see cref="_options.Filename"/> from latest commit 
        /// </summary>
        /// <returns>
        /// File contents as a string
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If repository has no commits
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If no file with name <see cref="_options.Filename"/> is not in the latest commit's tree
        /// </exception>
        private string GetChangelogText()
        {
            Commit head = PipelineContext.Repository.Lookup("HEAD") as Commit;
            if (head == null)
            {
                throw new InvalidOperationException($"Repository has no commits");
            }
            GitObject headChangelogGitObject = head[_options.FileName]?.Target;
            if (headChangelogGitObject == null)
            {
                throw new InvalidOperationException($"No file with name \"{_options.FileName}\"");
            }

            return ((Blob)headChangelogGitObject).ReadAsString();
        }
    }
}
