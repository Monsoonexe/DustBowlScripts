using CommandLine;
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
            workingInfo.hashname = ParseString();
            Consume(',');

            Consume("category_hashname=");
            workingInfo.category_hashname = ParseString();
            Consume(',');

            Consume("ped_type=");
            workingInfo.ped_type = ParseString();
            Consume(',');

            Consume("is_multiplayer=");
            workingInfo.is_multiplayer = ParseBool();
            Consume(',');

            Consume("category_hash=");
        }

        private bool ParseBool()
        {
            string token;

            if (Peek() == 't') // true
            {
                // parse 'true'
                for (int i = 4; i > 0; --i)
                    sb.Append(Advance());

                token = sb.ToStringAndClear();
                if (token != "true")
                    throw new ParseException("Parse Bool expected 'true' but got " + token);
                return true;
            }
            else if (Peek() == 'f') // false
            {
                // parse 'false'
                for (int i = 5; i > 0; --i)
                    sb.Append(Advance());

                token = sb.ToStringAndClear();
                if (token != "false")
                    throw new ParseException("Parse Bool expected 'false' but got " + token);
                return false;
            }
            else // error
            {
                for (int i = 5; i >= 0; --i)
                    sb.Append(Advance());

                token = sb.ToStringAndClear();
                throw new ParseException("Parse Bool expected 'true' or 'false' but got " + token);
            }
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
