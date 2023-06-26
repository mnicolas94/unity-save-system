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
            var doNotPersistAttributeType = typeof(DoNotPersistAttribute);
            
            var fields = source.GetType().GetRuntimeFields();
            foreach (var fieldInfo in fields)
            {
                var persistent = fieldInfo.CustomAttributes.All(
                    att => att.AttributeType != doNotPersistAttributeType);
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