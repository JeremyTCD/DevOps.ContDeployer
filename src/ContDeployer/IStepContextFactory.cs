using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer
{
    public interface IStepContextFactory
    {
        void LoadTypes();
        IStepContext Build();
    }
}
