using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.MSBuild
{
    public interface IMSBuildClient
    {
        void Build(string projectOrSlnFile = null, string switches = null);
    }
}
