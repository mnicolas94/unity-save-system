using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SaveSystem.Attributes;
using Utils;

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
            
            var fields = sourceType.GetRuntimeFields();
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
    }
}