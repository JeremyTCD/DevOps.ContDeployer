using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Composition;
using System.Collections.Generic;
using LibGit2Sharp;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace JeremyTCD.ContDeployer.Plugin.LogMetadataFactory
{
    [Export(typeof(IPlugin))]
    public class LogMetadataFactory : PluginBase
    {
        public override IDictionary<string, object> DefaultConfig { get; set; } = new Dictionary<string, object>
        {
            { "fileName", "changelog" },
            { "branch", "master" }
        };

        public override void Run(IDictionary<string, object> config, PipelineContext context, ILogger logger, LinkedList<PipelineStep> steps)
        {
            config = CombineConfigs(config, DefaultConfig);
            string fileName = (string) config["filename"];
            string branch = (string)config["branch"];

            // TODO what if you're working another branch
            List<Commit> commits = context.
                Repository.
                Commits.
                QueryBy(new CommitFilter() { SortBy = CommitSortStrategies.Topological }).
                ToList();

            if (commits.Count == 0)
            {
                throw new Exception($"{nameof(LogMetadataFactory)}: Repository has no commits");
            }

            // Get trees for head and previous commit
            Commit head = commits[0];
            Tree newTree = head.Tree;
            Commit previous = commits.Count == 1 ? null : commits[1];
            Tree oldTree = previous?.Tree;

            // Check whether changelog has been added or modified
            bool changelogChanged = false;
            if(commits.Count == 1)
            {
                changelogChanged = newTree.ToList().Any(treeEntry => treeEntry.Path == fileName);
            }
            else 
            {
                TreeChanges changes = context.Repository.Diff.Compare<TreeChanges>(oldTree, newTree, new string[] { fileName });
                changelogChanged = changes.
                    ToList().
                    Any(treeEntryChanges => treeEntryChanges.Path == fileName && (treeEntryChanges.Status == ChangeKind.Modified || treeEntryChanges.Status == ChangeKind.Added));
            }
            if (!changelogChanged)
            {
                return;
            }


            Console.WriteLine(newTree.Id);
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
        }
    }
}
