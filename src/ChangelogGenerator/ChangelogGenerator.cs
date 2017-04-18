using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using LibGit2Sharp.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public class ChangelogGenerator : IPlugin
    {
        private PipelineContext _pipelineContext { get; set; }
        private StepContext _stepContext { get; set; }
        private ChangelogGeneratorOptions _options { get; set; }

        /// <summary>
        /// Generates <see cref="Changelog"/> and inserts it into <see cref="PipelineContext.SharedData"/>.
        /// </summary>
        /// <param name="pipelineContext"></param>
        /// <param name="stepContext"></param>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="StepContext.Options"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="_options.Filename"/> is an empty 
        /// </exception>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            _options = stepContext.Options as ChangelogGeneratorOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(ChangelogGeneratorOptions)} required");
            }

            _pipelineContext = pipelineContext;
            _stepContext = stepContext;

            // TODO checkout branch specified in options
            // Get changelog text
            string changelogText = GetChangelogText();
            if (string.IsNullOrEmpty(changelogText))
            {
                throw new InvalidOperationException($"File with name \"{_options.FileName}\" is empty");
            }

            // Build changelog
            ChangelogFactory changelogFactory = new ChangelogFactory();
            Changelog changelog = changelogFactory.Build(_options.Pattern, changelogText);

            _pipelineContext.SharedData[nameof(Changelog)] = changelog;
            _stepContext.Logger.LogInformation($"{nameof(Changelog)} generated");
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
            Commit head = _pipelineContext.Repository.Lookup<Commit>("HEAD");
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
