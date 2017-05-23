using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild
{
    public interface IMSBuildClient
    {
        void Build(string projectOrSlnFile = null, string switches = null);
    }
}
