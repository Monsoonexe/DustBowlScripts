using CommandLine;
using System;
using System.IO;

namespace HashParser
{
    internal class Program
    {
        public static LaunchArguments LaunchArguments { get; private set; }

        // hex value == 0x123456789 (2 directives and 9 digits)

        static void Main(string[] args)
        {
            var parsedArgs = ParseCommandLineArguments(args);

            if (LaunchArguments == null)
            {
                // print usage
                PrintHelpText(parsedArgs);
                PressKeyToExitProgram();
                return;
            }
            else if (LaunchArguments.SourceFilePath != null)
            {
                RunFile(LaunchArguments.SourceFilePath, LaunchArguments.OutputFilePath);

                // exit prompt
                if (LaunchArguments.NoExit)
                {
                    PressKeyToExitProgram();
                }
            }
            else
            {
                // RunPrompt();
            }
        }

        private static void PressKeyToExitProgram()
        {
            Console.Write("\r\nPress any key to exit...\r\n>> ");
            Console.ReadKey();
        }


        private static ParserResult<LaunchArguments> ParseCommandLineArguments(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<LaunchArguments>(args);
            LaunchArguments = result.Value;
            return result;
        }

        private static void PrintHelpText(ParserResult<LaunchArguments> parsedArgs)
        {
            Print(GetHelpText(parsedArgs));
        }

        private static string GetHelpText(ParserResult<LaunchArguments> res)
        {
            return CommandLine.Text.HelpText.AutoBuild(res);
        }
        public static void Print(string value)
            => Console.WriteLine(value);

        public static void PrintFormat(string format, string value)
            => Print(string.Format(format, value));

        /// <summary>
        /// From file.
        /// </summary>
        /// <param name="srcPath"></param>
        private static void RunFile(string srcPath, string destPath)
        {
            if (string.IsNullOrEmpty(destPath))
                destPath = Path.GetTempFileName();

            using (var inFile = File.OpenRead(srcPath))
            using (var inStream = new BufferedStream(inFile))
            using (var outFile = File.OpenWrite(destPath))
            using (var outStream = new BufferedStream(outFile))
            {
                Run(inStream, outStream);
            }

            // open it for inspection
            System.Diagnostics.Process.Start(destPath);
        }

        private static void Run(Stream src, Stream dest)
        {
            var scanner = new CsvParser(src);
            var clothing = scanner.Parse();

            var sorter = new Sorter(clothing);
            var sortedClothing = sorter.Sort();

            var output = new LuaWriter(dest);
            output.WriteVariable("clothes_list", sortedClothing);
        }
    }
}
