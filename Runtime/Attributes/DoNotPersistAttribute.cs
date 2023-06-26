using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaveSystem.Attributes
{
    public class DoNotPersistAttribute : Attribute
    {
        private List<string> _fields;

        public ReadOnlyCollection<string> Fields => _fields.AsReadOnly();

        public DoNotPersistAttribute()
        {
        }

        public DoNotPersistAttribute(params string[] fields)
        {
            _fields = new List<string>(fields);
        }
    }
}