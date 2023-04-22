using CommandLine;

namespace HashParser
{
    internal class LaunchArguments
    {
        [Option('i', nameof(SourceFilePath), HelpText = "The source file to load.")]
        public string SourceFilePath { get; set; }

        [Option('o', nameof(OutputFilePath), HelpText = "The destination file to write to.")]
        public string OutputFilePath { get; set; }

        [Option(nameof(NoExit), HelpText = "Should the terminal remain open after a script has been run?")]
        public bool NoExit { get; set; } = false;
    }
}
