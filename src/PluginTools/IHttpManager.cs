using System.Net.Http;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IHttpManager
    {
        HttpResponseMessage Invoke(HttpRequestMessage requestMessage, bool executeOnDryRun = false);
    }
}
