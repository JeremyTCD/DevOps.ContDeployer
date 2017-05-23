using Semver;

namespace JeremyTCD.PipelinesCE.Plugin.Changelog
{
    public class Version : IVersion
    {
        public string Raw { get; set; }
        public SemVersion SemVersion { get; set; }
        public string Notes { get; set; }

        public int CompareTo(IVersion other)
        {
            int compareResult = SemVersion.CompareTo(other.SemVersion);

            return compareResult == 0 ? 0 :
                compareResult == -1 ? 1 : -1;
        }
    }
}
