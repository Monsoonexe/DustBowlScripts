using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HashParser
{
    /// <summary>
    /// Special-purpose writer.
    /// </summary>
    internal class LuaWriter : IDisposable
    {
        #region Constants

        private const string FalseString = "false";
        private const string TrueString = "true";

        #endregion Constants

        private readonly TextWriter writer;
        private int tabDepth = 0;

        public LuaWriter(Stream dest)
        {
            writer = new StreamWriter(dest);
        }

        private IEnumerable<FieldInfo> GetProperties(ClothingInfo animInfo)
        {
            // get them
            return null;
        }

        public void WriteVariable(string identifier, List<List<List<List<ClothingInfo>>>> clothing)
        {
            // emit lua
            WriteAssignment(identifier); // clothes_list = 
            WriteStartObject(); // {
            WriteSexes(clothing); // ...
            WriteEndObject(); // }
        }

        private void WriteSexes(List<List<List<List<ClothingInfo>>>> clothing)
        {
            foreach (var sex in clothing)
            {
                // emit lua
                WriteKey(sex[0][0][0].ped_type); // ['female']
                WriteAssignment(); // = 
                WriteStartObject(); // {
                WriteCategories(sex); // ...
                WriteEndObject(); // }

                // write comma on all but last
                if (!IsLastItem(clothing, sex))
                    WriteNextArrayElement();
            }
        }

        private void WriteCategories(List<List<List<ClothingInfo>>> sex)
        {
            foreach (var category in sex)
            {
                // emit lua
                WriteKey(category[0][0].category_hashname); // ['accessories']
                WriteAssignment();
                WriteStartObject();
                WriteModels(category);
                WriteEndObject();

                // write comma on all but last
                if (!IsLastItem(sex, category))
                    WriteNextArrayElement();
            }
        }

        private void WriteModels(List<List<ClothingInfo>> category)
        {
            for (int i = 0; i < category.Count; ++i)
            {
                // iterator
                var model = category[i];

                // emit lua
                WriteIndex(i); // [13]
                WriteAssignment(); // = 
                WriteStartObject(); // {
                WriteVariations(model); // ...
                WriteEndObject(); // }

                // write comma on all but last
                if (!IsLastItem(category, model))
                    WriteNextArrayElement();
            }
        }

        private void WriteVariations(List<ClothingInfo> variations)
        {
            for (int i = 0; i < variations.Count; ++i)
            {
                // iterator
                var variation = variations[i];

                // emit lua
                WriteIndex(i); // [1]
                WriteAssignment(); // =
                WriteStartObject(); // {
                Write(variation); // ...
                WriteEndObject(); // }

                // write comma on all but last
                if (!IsLastItem(variations, variation))
                    WriteNextArrayElement();
            }
        }

        private void WriteAssignment(string lhs) => Write(lhs + " = ");

        private void WriteAssignment() => Write(" = ");

        private void WriteStartObject()
        {
            Write("{");
            WriteNewLineIndented();
        }

        private void WriteEndObject()
        {
            WriteNewLineUnindented();
            Write("}");
        }

        private void WriteNextArrayElement()
        {
            Write(",");
            WriteNewLine();
        }

        private void Write(string v) => writer.Write(v);

        private void WriteKey(string identifier)
        {
            writer.Write($"['{identifier}']");
        }

        private void WriteIndex(int index) => Write($"[{index}]");

        public void Write(ClothingInfo cloth)
        {
            WriteProperty(nameof(cloth.category_hash), cloth.category_hash);
            WriteNextArrayElement();
            WriteProperty(nameof(cloth.category_hash_dec_signed), cloth.category_hash_dec_signed);
            WriteNextArrayElement();
            WriteProperty(nameof(cloth.category_hashname), cloth.category_hashname);
            WriteNextArrayElement();
            WriteProperty(nameof(cloth.hash), cloth.hash);
            WriteNextArrayElement();
            WriteProperty(nameof(cloth.hash_dec_signed), cloth.hash_dec_signed);
            WriteNextArrayElement();
            WriteProperty(nameof(cloth.hashname), cloth.hashname);
            WriteNextArrayElement();
            WriteBoolProperty(nameof(cloth.is_multiplayer), cloth.is_multiplayer);
            WriteNextArrayElement();
            WriteProperty(nameof(cloth.ped_type), cloth.ped_type);
        }

        private void WriteBoolProperty(string name, bool value)
        {
            WriteKey(name);
            WriteAssignment();
            Write(value ? TrueString : FalseString);
        }

        private void WriteProperty(string name, string value)
        {
            WriteKey(name);
            WriteAssignment();
            Write($"'{value}'");
        }

        private void WriteProperty(string name, long value)
        {
            WriteKey(name);
            WriteAssignment();
            Write(value.ToString());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void WriteIndent()
        {
            for (int i = tabDepth; i > 0; i--)
                writer.Write("\t");
        }

        private void WriteNewLine()
        {
            writer.Write("\r\n");
            WriteIndent();
        }

        private void WriteNewLineIndented()
        {
            writer.Write("\r\n");
            Indent();
            WriteIndent();
        }

        private void WriteNewLineUnindented()
        {
            writer.Write("\r\n");
            Unindent();
            WriteIndent();
        }

        private void Indent() => tabDepth++;

        private void Unindent() => tabDepth--;

        private static bool IsLastItem<T>(List<T> list, T item)
            where T : class
            => ReferenceEquals(list[list.Count - 1], item);

    }
}
