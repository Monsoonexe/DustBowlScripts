using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HashParser
{
    internal class Program
    {
        public static LaunchArguments LaunchArguments { get; private set; }
        private static bool hadError = false;

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

            try
            {
                using (var inStream = File.OpenRead(srcPath))
                using (var outStream = File.OpenWrite(destPath))
                {
                    Run(inStream, outStream);
                }

                // print success
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Run(Stream src, Stream dest)
        {
            // read
            var scanner = new CsvParser(src);
            var clothing = scanner.Parse();

            if (hadError)
                return; // syntax error

            var sorter = new Sorter(clothing);
            sorter.Sort();

            if (hadError)
                return; // syntax error

            var output = new OutputWriter(clothing, dest);
            output.WriteAll();
        }
    }
}
