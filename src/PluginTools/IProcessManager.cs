namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IProcessManager
    {
        int Execute(string fileName, string arguments, int timeoutMillis = int.MaxValue);
    }
}
