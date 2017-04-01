using JeremyTCD.DevOps.ContDeployer;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace JeremyTCD.DevOps.TestPlugin
{
    [Export(typeof(IPlugin))]
    public class TestPlugin : IPlugin
    {
        public void Execute()
        {
        }
    }
}
