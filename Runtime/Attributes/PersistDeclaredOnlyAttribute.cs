using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaveSystem.Attributes
{
    /// <summary>
    /// Mark a class to only persist declared fields and ignore any inherited ones.
    /// </summary>
    public class PersistDeclaredOnlyAttribute : Attribute
    {
        public PersistDeclaredOnlyAttribute()
        {
        }
    }
}