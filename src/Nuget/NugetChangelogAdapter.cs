using JeremyTCD.ContDeployer.PluginTools;
using NuGet.Commands;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JeremyTCD.ContDeployer.Plugin.Nuget
{
    public class NugetChangelogAdapter : PluginBase
    {
        private IListCommandRunner _listCommandRunner { get; }
        private NugetChangelogAdapterOptions _options { get; }


        public NugetChangelogAdapter(IPipelineContext pipelineContext, IStepContext stepContext, IListCommandRunner listCommandRunner) :
            base(pipelineContext, stepContext)
        {
            _options = stepContext.Options as NugetChangelogAdapterOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(NugetChangelogAdapterOptions)} required");
            }

            _listCommandRunner = listCommandRunner;
        }

        public override void Run()
        {
        }

        private void GetPackageVersions()
        {
            IList<PackageSource> packageSources = _options.
                Sources.
                Select(s => new PackageSource(s)).
                ToList();

            // TODO don't even need ListCommandRunenr since it just prints, we want the actual values
            ListArgs args = new ListArgs(new List<string>{_options.PackageName}, 
                packageSources, 
                NullSettings.Instance, // Not used by ListCommandRunner
                null, // TODO nuget has its own ilogger class, figure out how to use standard logger
                null, // TODO nuget has its own ilogger class, figure out how to use standard logger
                true, 
                "No packaged", // TODO move these to strings.resx, copy actual strings from nuget.client
                "License url: {0}", 
                "Not supported",
                true, true, true, CancellationToken.None);

            _
        }
    }
}
