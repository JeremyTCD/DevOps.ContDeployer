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

        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            _options = stepContext.Options as ChangelogGeneratorOptions;

            if(_options == null)
            {
                throw new InvalidOperationException($"{nameof(ChangelogGeneratorOptions)} required");
            }

            _pipelineContext = pipelineContext;
            _stepContext = stepContext;

            // TODO what if you're working another branch
            // Get changelog text
            string headChangelogText = GetHeadChangelogText();
            if (headChangelogText == null)
            {
                return;
            }
            string previousChangelogText = GetPreviousChangelogText();

            // Check if changelog has changed
            if (headChangelogText == previousChangelogText)
            {
                _stepContext.Logger.LogInformation($"No changes to changelog: {_options.FileName}");
                return;
            }

            // Build changelog metadata
            ChangelogFactory changelogMetadataFactory = new ChangelogFactory();
            Changelog headChangelogMetadata = changelogMetadataFactory.Build(_options.Pattern, headChangelogText);
            Changelog previousChangelogMetadata = previousChangelogText != null ?
                changelogMetadataFactory.Build(_options.Pattern, previousChangelogText) : null;

            // Diff changelog metadata
            Changelog diff = headChangelogMetadata.Diff(previousChangelogMetadata);

            if (diff.AddedVersions.Count > 1)
            {
                throw new InvalidOperationException($"Cannot add more than 1 version at a time. Deploy manually.");
            }

            if (diff.RemovedVersions.Count > 0)
            {
                // Removal does not make sense since certain operations (like publishing
                // to a package manager) have permanent side effects
                throw new InvalidOperationException($"Cannot remove versions. Deploy manually.");
            }

            _pipelineContext.SharedData[nameof(Changelog)] = diff;
            _stepContext.Logger.LogInformation($"{nameof(Changelog)} generated");
        }

        private string GetHeadChangelogText()
        {
            Commit head = _pipelineContext.Repository.Lookup<Commit>("HEAD");
            if (head == null)
            {
                throw new InvalidOperationException($"Repository has no commits");
            }
            GitObject headChangelogGitObject = head[_options.FileName]?.Target;
            if (headChangelogGitObject == null)
            {
                throw new InvalidOperationException($"No file with name: {_options.FileName}");
            }

            return ((Blob)headChangelogGitObject).ReadAsString();
        }

        private string GetPreviousChangelogText()
        {
            Commit previous = _pipelineContext.Repository.Lookup<Commit>("HEAD^");
            if (previous == null)
            {
                _stepContext.Logger.LogInformation($"First commit");
                return null;
            }
            GitObject previousChangelogGitObject = previous[_options.FileName]?.Target;
            if (previousChangelogGitObject == null)
            {
                _stepContext.Logger.LogInformation($"First commit for: {_options.FileName}");
                return null;
            }

            return ((Blob)previousChangelogGitObject).ReadAsString();
        }
    }
}
