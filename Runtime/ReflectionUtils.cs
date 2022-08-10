using System.Reflection;

namespace SaveSystem
{
    public static class ReflectionUtils
    {
        public static void CopyTo(object source, object destiny)
        {
            var fields = source.GetType().GetRuntimeFields();
            foreach (var fieldInfo in fields)
            {
                var value = fieldInfo.GetValue(source);
                fieldInfo.SetValue(destiny, value);
            }
        }
    }
}