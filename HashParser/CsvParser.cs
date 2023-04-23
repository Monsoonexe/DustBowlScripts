using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HashParser
{
    internal class CsvParser : IDisposable
    {
        private readonly StringBuilder sb = new StringBuilder();
        private readonly char[] buffer = new char[1024];
        private readonly StreamReader reader;
        private readonly ClothingInfo workingInfo = new ClothingInfo();
        private string previousLine;
        private int line = 1;

        public bool IsAtEnd { get => reader.EndOfStream; }

        public List<ClothingInfo> Clothing { get; private set; } = new List<ClothingInfo>(8);

        public CsvParser(Stream stream)
        {
            reader = new StreamReader(stream);
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        public List<ClothingInfo> Parse()
        {
            // consume headers
            ConsumeLine("local cloth_hash_names = {");

            while (!IsAtEnd && Peek() != '}')
            {
                // this is a hella hack to know that we have reached the end of the records
                try
                {
                     Consume('{');
                }
                catch
                {
                    break;
                }
                ScanLine();
                Clothing.Add(workingInfo.Clone());
                line++;
            }

            return Clothing;
        }

        private void ScanLine()
        {
            Consume("hashname=");
            workingInfo.hashname = ParseString();

            Consume("category_hashname=");
            workingInfo.category_hashname = ParseString();

            Consume("ped_type=");
            workingInfo.ped_type = ParseString();

            Consume("is_multiplayer=");
            workingInfo.is_multiplayer = ParseBool();

            Consume("category_hash=");
            workingInfo.category_hash = ParseHex();

            Consume("hash=");
            workingInfo.hash = ParseHex();

            Consume("hash_dec_signed=");
            workingInfo.hash_dec_signed = ParseNumber(',');

            Consume("category_hash_dec_signed=");
            workingInfo.category_hash_dec_signed = ParseNumber('}');

            // end object
            Consume(',');
        }

        private long ParseNumber(char terminator)
        {
            string token = ReadValue(terminator);

            try
            {
                return long.Parse(token);
            }
            catch (Exception ex)
            {
                throw new ParseException($"Line {line}: Could not convert {token} to a number.", ex);
            }
        }

        private long ParseHex()
        {
            Consume("0x");
            string token = ReadValue();

            try
            {
                return Convert.ToInt64(token, 16);
            }
            catch (Exception ex)
            {
                throw new ParseException($"Line {line}: Could not convert {token} to hex.", ex);
            }
        }

        private bool ParseBool()
        {
            string token;

            if (Peek() == 't') // true
            {
                // parse 'true'
                token = ReadValue();
                if (token != "true")
                    throw new ParseException("Parse Bool expected 'true' but got " + token);
                return true;
            }
            else if (Peek() == 'f') // false
            {
                // parse 'false'
                token = ReadValue();
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
            string value = ReadValue();

            // trim leading and trailing double quotes ""
            value = value.Substring(1, value.Length - 2);
            return value;
        }

        private string ReadValue() => ReadValue(',');

        private string ReadValue(char terminatingChar)
        {
            while (!IsAtEnd && Peek() != terminatingChar)
                sb.Append(Advance());

            if (IsAtEnd)
                throw new ParseException($"Line {line}: Hit end of file before seeing {terminatingChar}.");

            return sb.ToStringAndClear();
        }

        private char Peek()
        {
            if (IsAtEnd)
                return '\0';
            char result = (char)reader.Peek();
            return result;
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

        private void ConsumeEmptyLines()
        {
            while (string.IsNullOrWhiteSpace(previousLine))
                AdvanceLine();
        }

        private void Consume(char target)
        {
            while (!IsAtEnd && Advance() != target);

            if (IsAtEnd)
                throw new ParseException($"Line {line}: Hit end of file before finding " + target);
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
