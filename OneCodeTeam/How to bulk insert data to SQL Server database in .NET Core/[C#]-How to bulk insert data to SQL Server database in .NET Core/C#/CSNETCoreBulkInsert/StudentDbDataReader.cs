using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.Common;
using System.Reflection;
namespace CSNETCoreBulkInsert
{
    public class StudentDbDataReader : DbDataReader
    {

        private List<Student> Students;
        private List<Student>.Enumerator _enumerator;
        protected List<string> Fields;
        protected List<Type> FieldTypes;
        protected bool isClosed;
        private Student _current;

        public StudentDbDataReader(List<Student>  list)
        {
            Students = list;
            _enumerator = list.GetEnumerator();
            IEnumerable<PropertyInfo> listProperty = typeof(Student).GetRuntimeProperties();
            Fields = new List<string>();
            FieldTypes = new List<Type>();
            foreach (PropertyInfo p in listProperty)
            {
                Fields.Add(p.Name);
                FieldTypes.Add(p.PropertyType);
            }
        }

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override int Depth => throw new NotImplementedException();

        public override int FieldCount => Fields.Count;

        public override bool HasRows => Students.Count>0;

        public override bool IsClosed => isClosed;

        public override int RecordsAffected => -1;

        public override bool GetBoolean(int ordinal)
        {
            return (bool)GetValue(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return (byte)GetValue(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            return (char)GetValue(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            return GetFieldType(ordinal).Name;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return (DateTime)GetValue(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return (decimal)GetValue(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return (double)GetValue(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            return Students.GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            return FieldTypes[ordinal];
        }

        public override float GetFloat(int ordinal)
        {
            return (float)GetValue(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return (Guid)GetValue(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return (short)GetValue(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return (int)GetValue(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return (long)GetValue(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return Fields[ordinal];
        }

        public override int GetOrdinal(string name)
        {
            return Fields.IndexOf(name);
        }

        public override string GetString(int ordinal)
        {
            return (string)GetValue(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return typeof(Student).GetRuntimeProperty(GetName(ordinal)).GetValue(_current);           
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            bool result = _enumerator.MoveNext();
            _current = result ? _enumerator.Current : new Student();
            return result;
        }
    }
}