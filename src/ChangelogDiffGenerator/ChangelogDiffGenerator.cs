using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using LibGit2Sharp.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator
{
    public class ChangelogDiffGenerator : PluginBase
    {
        private PipelineContext _pipelineContext { get; set; }
        private PipelineStepContext _pipelineStepContext { get; set; }
        private ChangelogDiffGeneratorOptions _options { get; set; }

        public override void Run(PipelineContext pipelineContext, PipelineStepContext pipelineStepContext)
        {
            _options = pipelineStepContext.Options as ChangelogDiffGeneratorOptions;
            _pipelineContext = pipelineContext;
            _pipelineStepContext = pipelineStepContext;

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
                _pipelineStepContext.Logger.LogInformation($"No changes to changelog: {_options.FileName}");
                return;
            }

            // Build changelog metadata
            ChangelogMetadataFactory changelogMetadataFactory = new ChangelogMetadataFactory();
            ChangelogMetadata headChangelogMetadata = changelogMetadataFactory.Build(_options.Pattern, headChangelogText);
            ChangelogMetadata previousChangelogMetadata = previousChangelogText != null ?
                changelogMetadataFactory.Build(_options.Pattern, previousChangelogText) : null;

            // Diff changelog metadata
            ChangelogDiff diff = headChangelogMetadata.Diff(previousChangelogMetadata);

            if (diff.AddedVersions.Count > 1)
            {
                throw new InvalidOperationException($"Cannot add more than 1 version at a time. Deploy manually.");
            }

            if (diff.RemovedVersions.Count > 0)
            {
                // Removal does not make sense since certain objects are permanent
                throw new InvalidOperationException($"Cannot remove versions. Deploy manually.");
            }

            _pipelineContext.SharedData[nameof(ChangelogDiff)] = diff;
            _pipelineStepContext.Logger.LogInformation($"{nameof(ChangelogDiff)} generated");
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
                _pipelineStepContext.Logger.LogInformation($"First commit");
                return null;
            }
            GitObject previousChangelogGitObject = previous[_options.FileName]?.Target;
            if (previousChangelogGitObject == null)
            {
                _pipelineStepContext.Logger.LogInformation($"First commit for: {_options.FileName}");
                return null;
            }

            return ((Blob)previousChangelogGitObject).ReadAsString();
        }
    }
}
