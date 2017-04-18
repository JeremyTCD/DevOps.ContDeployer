using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    public class TagGeneratorChangelogAdapter : IPlugin
    {
        /// <summary>
        /// If <see cref="Changelog.AddedVersions"/> contains a <see cref="Version"/>,
        /// adds a <see cref="TagGenerator"/> pipeline step.
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object diffObject);
            Changelog diff = diffObject as Changelog;
            if(diff == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
            }

            if (diff.AddedVersions.Count == 1)
            {
                TagGeneratorOptions tagGeneratorOptions = new TagGeneratorOptions
                {
                    TagName = diff.AddedVersions.First().SemVersion.ToString()
                };
                Step tagGeneratorStep = new Step(nameof(TagGenerator), tagGeneratorOptions);
                pipelineContext.Steps.AddFirst(tagGeneratorStep);

                stepContext.Logger.LogInformation($"Version added to changelog, added {nameof(TagGenerator)} step");
            }
            else if (diff.AddedVersions.Count > 1)
            {
                throw new InvalidOperationException($"{nameof(Changelog)} should not have more than 1 added versions");
            }
            else
            {
                stepContext.Logger.LogInformation($"No versions added to changelog");
            }
        }
    }
}
