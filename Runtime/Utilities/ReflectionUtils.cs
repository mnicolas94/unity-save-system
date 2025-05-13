using System;
using System.Collections.Generic;
using System.Reflection;
using SaveSystem.Attributes;
using Unity.Properties;
using UnityEngine;

namespace SaveSystem.Utilities
{
    public static class ReflectionUtils
    {
        public static void CopyTo(object source, object destiny)
        {
            var sourceType = source.GetType();
            
            var fields = GetPersistentFields(sourceType);
            foreach (var fieldInfo in fields)
            {
                var value = fieldInfo.GetValue(source);
                fieldInfo.SetValue(destiny, value);
            }
        }
        
        public static List<FieldInfo> GetPersistentFields(Type type)
        {
            var persistentFields = new List<FieldInfo>();
            var allowedFields = new List<FieldInfo>();
            
            CopyAllowedFields(type, allowedFields);
            
            // get fields to ignore
            var ignoreFields = new List<string>();
            var doNotPersistAttribute = type.GetCustomAttribute<DoNotPersistAttribute>();
            if (doNotPersistAttribute != null)
            {
                ignoreFields.AddRange(doNotPersistAttribute.Fields);
            }
            CopyPersistentFields(persistentFields, allowedFields, ignoreFields);
            
            return persistentFields;
        }

        private static void CopyAllowedFields(Type type, List<FieldInfo> allowedFields)
        {
            var persistDeclaredOnly = type.GetCustomAttribute<PersistDeclaredOnlyAttribute>() != null;
            if (persistDeclaredOnly)
            {
                allowedFields.AddRange(type.GetRuntimeFields());
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

                    var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                       BindingFlags.DeclaredOnly;
                    var fields = type.GetFields(bindingFlags);
                    foreach (var field in fields)
                    {
                        if (onlyFields.Count == 0 || onlyFields.Contains(field.Name))
                        {
                            allowedFields.Add(field);
                        }
                    }
                    
                    type = type.BaseType;
                }
            }
        }

        private static void CopyPersistentFields(List<FieldInfo> destiny, List<FieldInfo> fields,
            List<string> ignoreFieldsNames)
        {
            foreach (var field in fields)
            {
                var doPersist = Attribute.IsDefined(field, typeof(SerializeField));
                doPersist |= !Attribute.IsDefined(field, typeof(NonSerializedAttribute)) && field.IsPublic;
                doPersist &= !Attribute.IsDefined(field, typeof(DoNotPersistAttribute));
                doPersist &= !ignoreFieldsNames.Contains(field.Name);
                if (doPersist)
                {
                    destiny.Add(field);
                }
            }
        }
    }
}