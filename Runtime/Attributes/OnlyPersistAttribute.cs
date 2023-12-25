using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaveSystem.Attributes
{
    /// <summary>
    /// Define a list of fields to be the only ones to persist by the save system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OnlyPersistAttribute : Attribute
    {
        private List<string> _fields;

        public ReadOnlyCollection<string> Fields => _fields.AsReadOnly();

        public OnlyPersistAttribute(params string[] fields)
        {
            _fields = new List<string>(fields);
        }
    }
}