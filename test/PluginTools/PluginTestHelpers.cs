using LibGit2Sharp;
using NSubstitute;
using System.Collections.Generic;
using System.IO;

namespace JeremyTCD.ContDeployer.PluginTools.Tests
{
    /// <summary>
    /// Helper functions for testing plugins
    /// </summary>
    public class PluginTestHelpers
    {
        public static IStepContext CreateMockStepContext(IPluginOptions options = null)
        {
            IStepContext stepContext = Substitute.For<IStepContext>();
            stepContext.Options.Returns(options);

            return stepContext;
        }

        public static Blob CreateMockBlob(string value)
        {
            Blob blob = Substitute.For<Blob>();
            blob.GetContentStream().Returns(CreateStreamFromString(value));

            return blob;
        }

        public static TreeEntry CreateMockTreeEntry(Blob blob)
        {
            TreeEntry treeEntry = Substitute.For<TreeEntry>();
            treeEntry.Target.Returns(blob);

            return treeEntry;
        }

        public static Commit CreateMockCommit(string fileName = null, TreeEntry treeEntry = null)
        {
            Commit commit = Substitute.For<Commit>();
            if (fileName != null)
            {
                commit[fileName].Returns(treeEntry);
            }

            return commit;
        }

        public static IRepository CreateMockRepository(string commitish = null, Commit commit = null)
        {
            IRepository repository = Substitute.For<IRepository>();
            if (commitish != null)
            {
                repository.Lookup<Commit>(commitish).Returns(commit);
            }

            return repository;
        }

        public static IDictionary<string, object> CreateSharedData(string key, object value)
        {
            IDictionary<string, object> sharedData = Substitute.For<IDictionary<string, object>>();
            sharedData[key].Returns(value);

            return sharedData;
        }

        public static IPipelineContext CreateMockPipelineContext(IDictionary<string, object> sharedData = null, 
            IRepository repository = null)
        {
            IPipelineContext pipelineContext = Substitute.For<IPipelineContext>();
            pipelineContext.Repository.Returns(repository);
            pipelineContext.SharedData.Returns(sharedData);
            return pipelineContext;
        }

        public static Stream CreateStreamFromString(string value)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}
