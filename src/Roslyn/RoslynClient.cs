using Microsoft.CodeAnalysis;

namespace Roslyn
{
    public class RoslynClient
    {
        public void CompileProject(string path)
        {
            // MSBuildWorkspace is not compatible with .net core
        }
    }
}
