using LibGit2Sharp;

namespace JeremyTCD.ContDeployer.Plugin.Git
{
    public interface IRepositoryFactory
    {
        IRepository Build(string path);
    }
}
