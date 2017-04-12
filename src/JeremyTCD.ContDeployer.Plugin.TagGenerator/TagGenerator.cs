using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    public class TagGenerator : PluginBase
    {
        public TagGeneratorOptions Options { get; set; }
        ILogger<TagGenerator> Logger { get; set; }

        public TagGenerator(TagGeneratorOptions options, ILogger<TagGenerator> logger, IRepository repository):
            base(repository)
        {
            Options = options;
            Logger = logger;
        }

        public override void Run(Dictionary<string, object> sharedData, LinkedList<PipelineStep> steps)
        {
            throw new NotImplementedException();
        }
    }
}
