using System;

namespace HashParser
{
    internal class ParseException : Exception
    {
        public ParseException(string context, string expected, string actual)
            : this($"Error during {context}: Expected '{expected}' but saw '{actual}'.")
        {
            // exists
        }

        public ParseException(string context, char expected, char actual)
            : this(context, expected.ToString(), actual.ToString())
        {
            // exists
        }

        public ParseException(string message) : base(message)
        {
            // exists
        }
    }
}
