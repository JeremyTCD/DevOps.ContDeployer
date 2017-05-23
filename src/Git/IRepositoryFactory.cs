using LibGit2Sharp;

namespace JeremyTCD.PipelinesCE.Plugin.Git
{
    public interface IRepositoryFactory
    {
        IRepository Build(string path);
    }
}
