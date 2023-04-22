using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HashParser
{
    internal class CsvScanner : IDisposable
    {
        private readonly StringBuilder sb = new StringBuilder();
        private readonly char[] buffer = new char[1024];
        private readonly StreamReader reader;
        private readonly ClothingInfo workingInfo = new ClothingInfo();
        private string previousLine;
        private int line = 1;

        public bool IsAtEnd { get => reader.EndOfStream; }

        public List<string> Lines { get; private set; }

        public CsvScanner(Stream stream)
        {
            reader = new StreamReader(new BufferedStream(stream));
        }

        public void Dispose()
        {
            reader.Dispose();

        }

        public List<string> Scan()
        {
            // TODO - 
            ConsumeHeaders();
            
            while (!IsAtEnd && Peek() != '}')
            {
                ConsumeWhiteSpace();
                ScanLine();
            }

            // read each line until the end
            // get an object definition: { ... }

            return Lines;
        }

        private void ScanLine()
        {
            // scan {
            if (!Match('{'))
                throw new ParseException("Line " + line, '{', Peek());

            Consume("hashname=");
            workingInfo.category_hashname = ParseString();
            Consume(',');


        }

        private string ParseString()
        {
            Consume('"');

            while (Peek() != '"')
                sb.Append(Advance());

            return sb.ToStringAndClear();
        }

        private char Peek()
        {
            if (IsAtEnd)
                return '\0';
            return (char)reader.Peek();
        }

        private char Advance()
        {
            return (char)reader.Read();
        }

        private string AdvanceLine()
        {
            line++;
            return previousLine = reader.ReadLine();
        }

        /// <summary>
        /// Consume everything until you hit a valid token.
        /// </summary>
        private void ConsumeHeaders()
        {
            // the first line of valid data
            ConsumeLine("local cloth_hash_names = {");
            ConsumeEmptyLines();
        }

        private void ConsumeEmptyLines()
        {
            while (string.IsNullOrWhiteSpace(previousLine))
                AdvanceLine();
        }

        private void ConsumeWhiteSpace()
        {
            char next;
            do next = Peek();
            while (next == ' ' || next == '\r' || next == '\t');
        }

        private void Consume(char target)
        {
            while (!IsAtEnd && Advance() != target);

            if (IsAtEnd)
                throw new ParseException("Hit end of file before finding " + target);
        }

        private void Consume(string target)
        {
            int i = 0;
            int length = target.Length;

            while (i < length)
            {
                if (Advance() != target[i++])
                {
                    i = 0;
                }
            }
        }

        /// <summary>
        /// Consume lines until we get a valid target.
        /// </summary>
        private void ConsumeLine(string target)
        {
            while (AdvanceLine() != target);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd || Peek() != expected)
                return false;

            Advance();
            return true;
        }

    }
}
