namespace JeremyTCD.ContDeployer.PluginTools
{
    public class StepFactory : IStepFactory
    {
        public IStep Build(string pluginName, IPluginOptions options)
        {
            return new Step(pluginName, options);
        }
    }
}
