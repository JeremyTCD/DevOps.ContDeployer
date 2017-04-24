namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IStepFactory
    {
        IStep Build(string pluginName, IPluginOptions options);
    }
}
