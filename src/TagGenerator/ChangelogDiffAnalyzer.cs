using JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    public class ChangelogDiffAnalyzer : PluginBase
    {
        ILogger<ChangelogDiffAnalyzer> Logger { get; set; }

        public ChangelogDiffAnalyzer(ILogger<ChangelogDiffAnalyzer> logger, IRepository repository):
            base(repository)
        {
            Logger = logger;
        }

        // TODO test this
        /// <summary>
        /// If <see cref="ChangelogDiff.AddedVersions"/> contains a <see cref="Version"/>,
        /// adds a <see cref="TagGenerator"/> pipeline step.
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        public override void Run(Dictionary<string, object> sharedData, LinkedList<PipelineStep> steps)
        {
            sharedData.TryGetValue(nameof(ChangelogDiff), out object diffObject);
            ChangelogDiff diff = diffObject as ChangelogDiff;
            if(diff == null)
            {
                throw new InvalidOperationException($"No {nameof(ChangelogDiff)} in {nameof(sharedData)}");
            }

            if (diff.AddedVersions.Count == 1)
            {
                Logger.LogInformation($"Version added to changelog");

                TagGeneratorOptions tagGeneratorOptions = new TagGeneratorOptions
                {
                    TagName = diff.AddedVersions.First().SemVersion.ToString()
                };
                PipelineStep tagGeneratorStep = new PipelineStep(nameof(TagGenerator), tagGeneratorOptions);
                steps.AddFirst(tagGeneratorStep);

                Logger.LogInformation($"Added {nameof(TagGenerator)} step");
            }
            else if (diff.AddedVersions.Count > 1)
            {
                throw new InvalidOperationException($"{nameof(ChangelogDiff)} should not have more than 1 added versions");
            }
            else
            {
                Logger.LogInformation($"Version added to changelog");
            }
        }
    }
}
