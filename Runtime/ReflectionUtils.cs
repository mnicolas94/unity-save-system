using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SaveSystem.Attributes;
using UnityEngine;

namespace SaveSystem
{
    public static class ReflectionUtils
    {
        public static void CopyTo(object source, object destiny)
        {
            var sourceType = source.GetType();
            
            // get the fields to ignore
            var ignoreFields = new List<string>();
            var att = sourceType.GetCustomAttribute<DoNotPersistAttribute>();
            if (att != null)
            {
                ignoreFields.AddRange(att.Fields);
            }
            
            var fields = GetSerializableFields(sourceType);
            foreach (var fieldInfo in fields)
            {
                var ignore = ignoreFields.Contains(fieldInfo.Name);
                if (ignore)
                {
                    continue;
                }

                var persistent = fieldInfo.GetCustomAttribute<DoNotPersistAttribute>() == null;
                if (!persistent)
                {
                    continue;
                }
                
                var value = fieldInfo.GetValue(source);
                fieldInfo.SetValue(destiny, value);
            }
        }
        
        public static List<FieldInfo> GetSerializableFields(Type type)
        {
            var serializableFields = new List<FieldInfo>();
            var persistDeclaredOnly = type.GetCustomAttribute<PersistDeclaredOnlyAttribute>() != null;
            
            if (persistDeclaredOnly)
            {
                serializableFields.AddRange(type.GetRuntimeFields());
            }
            else
            {
                var onlyFields = new List<string>();
                while (type != null)
                {
                    var att = type.GetCustomAttribute<OnlyPersistAttribute>();
                    if (att != null)
                    {
                        onlyFields.AddRange(att.Fields);
                    }
                    
                    var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                    var fields = type.GetFields(bindingFlags);
                    foreach (var field in fields)
                    {
                        // Check if the field is marked as serializable or is public
                        if (Attribute.IsDefined(field, typeof(SerializeField)) || 
                            (!Attribute.IsDefined(field, typeof(NonSerializedAttribute)) && field.IsPublic))
                        {
                            var doPersist = onlyFields.Count == 0 || onlyFields.Contains(field.Name);
                            if (doPersist)
                            {
                                serializableFields.Add(field);
                            }
                        }
                    }

                    type = type.BaseType;
                }
            }

            return serializableFields;
        }
    }
}