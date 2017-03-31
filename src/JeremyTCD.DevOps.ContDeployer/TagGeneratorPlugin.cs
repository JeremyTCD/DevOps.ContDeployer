using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace JeremyTCD.DevOps.ContDeployer
{
    [Export(typeof(IPlugin))]
    public class TagGeneratorPlugin : IPlugin
    {
        public void Execute()
        {
        }
    }
}
