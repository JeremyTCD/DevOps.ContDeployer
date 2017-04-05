using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Composition;
using System.Collections.Generic;
using LibGit2Sharp;

namespace JeremyTCD.ContDeployer.Plugin.LogMetadataFactory
{
    [Export(typeof(IPlugin))]
    public class LogMetadataFactory : PluginBase
    {
        public override IDictionary<string, object> DefaultConfig { get; set; } = new Dictionary<string, object>
        {
            { "fileName", "changelog.md" },
            { "branch", "master" }
        };

        public override void Run(IDictionary<string, object> config, PipelineContext context, LinkedList<PipelineStep> steps)
        {
            config = CombineConfigs(config, DefaultConfig);

            // TODO: create integration test with dummy folder so plugin can be stepped through

            // Check if changelog changed
            //  - get changed files using git show: Invoke-Git show --pretty="" --name-only $commit
            //  - check if changelog changed
            //  - if no return

            // Checkout branch so current changelog is head version

            // Checkout old changelog 
            //  - Just checkout head^ changelog since this system does not allow
            //    published stuff to be changed: Invoke -Git show $commit^:$file > 'changelogOld.md'

            // Build log metadata

            // generate a simple diff, which versions have changed/been added

            // Repo should come from context since its shared by all plugins, this way it can be injected
            //using (Repository repo = new Repository("path/to/your/repo"))
            //{
            //    CheckoutOptions checkoutOptions = new CheckoutOptions()
            //    {
            //        CheckoutModifiers
            //    }
            //    Commands.Checkout(repo, "master");

            //}




            throw new NotImplementedException();
        }
    }
}
