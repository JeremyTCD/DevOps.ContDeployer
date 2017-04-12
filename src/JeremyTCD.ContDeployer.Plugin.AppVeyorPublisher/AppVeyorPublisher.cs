using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Collections.Generic;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace JeremyTCD.ContDeployer.Plugin.AppVeyorPublisher
{
    public class AppVeyorPublisher : PluginBase
    {
        AppVeyorPublisherOptions Options { get; set; }
        ILogger<AppVeyorPublisher> Logger { get; set; }

        public AppVeyorPublisher(AppVeyorPublisherOptions options, ILogger<AppVeyorPublisher> logger, IRepository repository) : base(repository)
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
