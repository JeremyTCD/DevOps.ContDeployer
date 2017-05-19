using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace JeremyTCD.ContDeployer.Plugin.Changelog
{
    public class ChangelogPlugin : IPlugin
    {
        private IChangelogFactory _changelogFactory { get; set; }
        private IFileService _fileService { get; set; }

        /// <summary>
        /// Creates a <see cref="ChangelogPlugin"/> instance
        /// </summary>
        public ChangelogPlugin(IChangelogFactory changelogFactory, IFileService fileService) 
        {
            _fileService = fileService;
            _changelogFactory = changelogFactory;
        }

        /// <summary>
        /// Generates <see cref="Changelog"/> and inserts it into <see cref="IPipelineContext.SharedData"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IStepContext.Options"/> is not an instance of <see cref="ChangelogPluginOptions"/>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="ChangelogPluginOptions.File"/> does not exist
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="ChangelogPluginOptions.File"/> is empty 
        /// </exception>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            ChangelogPluginOptions options = stepContext.Options as ChangelogPluginOptions;

            if (options == null)
            {
                throw new InvalidOperationException($"{nameof(ChangelogPluginOptions)} required");
            }

            if (!_fileService.Exists(options.File))
            {
                throw new InvalidOperationException($"File \"{options.File}\" does not exist");
            }

            string changelogText = _fileService.ReadAllText(options.File);
            if (string.IsNullOrEmpty(changelogText))
            {
                throw new InvalidOperationException($"File with name \"{options.File}\" is empty");
            }

            // Build changelog
            IChangelog changelog = _changelogFactory.Build(options.Pattern, changelogText);

            pipelineContext.SharedData[nameof(Changelog)] = changelog;
            stepContext.Logger.LogInformation($"{nameof(Changelog)} generated");
        }
    }
}
