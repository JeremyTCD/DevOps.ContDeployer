namespace JeremyTCD.PipelinesCE.Core
{
    public class PipelinesCEOptions
    {
        public const string DefaultLogFile = "PipelinesCE.log";
        public const string DefaultProjectFile = "PipelinesCEConfig.csproj";
        public const string DefaultPipeline = "Default";
        public const bool DefaultVerbose = false;
        public const bool DefaultDebug = false;
        public const bool DefaultFileLogging = false;

        private string _logFile;
        private string _projectFile;
        private bool? _debug;
        private bool? _verbose;
        private string _pipeline;
        private bool? _fileLogging;

        public virtual bool FileLogging
        {
            get
            {
                return _fileLogging ?? DefaultFileLogging;
            }
            set { _fileLogging = value; }
        }

        public virtual string LogFile
        {
            get
            {
                return _logFile ?? DefaultLogFile;
            }
            set { _logFile = value; }
        }

        public virtual string ProjectFile
        {
            get
            {
                return _projectFile ?? DefaultProjectFile;
            }
            set { _projectFile = value; }
        }

        public virtual bool Debug
        {
            get
            {
                return _debug ?? DefaultDebug;
            }
            set { _debug = value; }
        }

        public virtual bool Verbose
        {
            get
            {
                return _verbose ?? DefaultVerbose;
            }
            set { _verbose = value; }
        }

        public virtual string Pipeline
        {
            get
            {
                return _pipeline ?? DefaultPipeline;
            }
            set { _pipeline = value; }
        }
    }
}
