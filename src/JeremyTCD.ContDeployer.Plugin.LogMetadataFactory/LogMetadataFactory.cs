using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Composition;
using System.Collections.Generic;
using LibGit2Sharp;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using LibGit2Sharp.Extensions;
using System.Text.RegularExpressions;
using Semver;

namespace JeremyTCD.ContDeployer.Plugin.LogMetadataFactory
{
    [Export(typeof(IPlugin))]
    public class LogMetadataFactory : PluginBase
    {
        public override IDictionary<string, object> DefaultConfig { get; set; } = new Dictionary<string, object>
        {
            { "fileName", "changelog.md" },
            { "branch", "master" },
            // TODO include prerelease semver patterns
            { "pattern", @"##[ \t]+(\d*\.\d*\.\d*)(.*?)(?=##[ \t]+\d*\.\d*\.\d*|$)"}
        };

        public override void Run(IDictionary<string, object> config, PipelineContext context, ILogger logger, LinkedList<PipelineStep> steps)
        {
            logger.LogInformation("=== Running LogMetadataFactory ===");

            config = CombineConfigs(config, DefaultConfig);
            string fileName = (string)config["fileName"];
            string branch = (string)config["branch"];
            string pattern = (string)config["pattern"];

            // TODO what if you're working another branch
            // Get changelog text
            string headChangelogText = GetHeadChangelogText(context.Repository, fileName, logger);
            if (headChangelogText == null)
            {
                return;
            }
            string previousChangelogText = GetPreviousChangelogText(context.Repository, fileName, logger);

            // Check if changelog has changed
            if (headChangelogText == previousChangelogText)
            {
                logger.LogInformation($"No changes to changelog: {fileName}");
                return;
            }

            // Build changelog metadata
            ChangelogMetadataFactory changelogMetadataFactory = new ChangelogMetadataFactory();
            ChangelogMetadata headChangelogMetadata = changelogMetadataFactory.Build(pattern, headChangelogText);
            ChangelogMetadata previousChangelogMetadata = previousChangelogText != null ?
                changelogMetadataFactory.Build(pattern, previousChangelogText): null;

            // Diff changelog metadata
            ChangelogMetadataDiff diff = headChangelogMetadata.Diff(previousChangelogMetadata);

            // if new version added, add taggenerator, githubreleasepublisher and wtv publisher
            // if tag incremented, add taggenerator, githubreleasepublisher and wtv publisher
            // if changes made, add githubreleasepublisher


            //Console.WriteLine(newTree.Id);
        }

        private string GetHeadChangelogText(IRepository repository, string fileName, ILogger logger)
        {
            Commit head = repository.Lookup<Commit>("HEAD");
            if (head == null)
            {
                logger.LogInformation($"Repository has no commits");
                return null;
            }
            GitObject headChangelogGitObject = head[fileName]?.Target;
            if (headChangelogGitObject == null)
            {
                logger.LogInformation($"No file with name: {fileName}");
                return null;
            }

            return ((Blob)headChangelogGitObject).ReadAsString();
        }

        private string GetPreviousChangelogText(IRepository repository, string fileName, ILogger logger)
        {
            Commit previous = repository.Lookup<Commit>("HEAD^");
            if (previous == null)
            {
                logger.LogInformation($"First commit");
                return null;
            }
            GitObject previousChangelogGitObject = previous[fileName]?.Target;
            if (previous == null)
            {
                logger.LogInformation($"First commit for: {fileName}");
                return null;
            }

            return ((Blob)previousChangelogGitObject).ReadAsString();
        }
    }
}
