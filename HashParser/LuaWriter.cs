using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HashParser
{
    internal class LuaWriter : IDisposable
    {
        private readonly TextWriter writer;
        private List<ClothingInfo> clothing;

        public LuaWriter(List<ClothingInfo> clothing, Stream dest)
        {
            this.clothing = clothing;
            writer = new StreamWriter(dest);
        }

        private IEnumerable<FieldInfo> GetProperties(ClothingInfo animInfo)
        {
            // get them
            return null;
        }

        public void WriteAll()
        {
            // TODO - 
        }

        public void Write(ClothingInfo animInfo)
        {
            // {
            //      props..
            // }
        }

        private void Write(IEnumerable<Property> info)
        {
            // prop,
            // prop,
            // prop
            // ...
        }

        private void Write(Property info)
        {
            switch (info.type)
            {
                case Property.EType.String:
                    WriteString(info);
                    break;
                case Property.EType.Number:
                    WriteNumber(info);
                    break;
                case Property.EType.Bool:
                    WriteBool(info);
                    break;
            }
        }

        private void WriteBool(Property prop)
        {
            // ['NAME'] = VALUE
        }

        private void WriteNumber(Property prop)
        {
            // ['NAME'] = VALUE

        }

        private void WriteString(Property prop)
        {
            // ['NAME'] = 'VALUE'

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private class Property
        {
            public readonly string name;
            public readonly EType type;
            public readonly object value;

            public Property(string name, EType type, object value)
            {
                this.name = name;
                this.type = type;
                this.value = value;
            }

            public enum EType
            {
                String = 0,
                Number = 1,
                Bool = 2
            }
        }
    }
}
