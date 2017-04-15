using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    public class TagGenerator : IPlugin
    {
        /// <summary>
        /// Tags head
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            // git tag Options.TagName
        }
    }
}
