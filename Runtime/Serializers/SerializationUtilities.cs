using Unity.Properties;

namespace SaveSystem.Serializers
{
    public static class SerializationUtilities
    {
        public static void CopyTo(object source, object destiny)
        {
            var visitor = new CopyToVisitor(destiny);
            PropertyContainer.Accept(visitor, ref source);
        }
    }
    
    public class CopyToVisitor : IPropertyBagVisitor, IPropertyVisitor
    {
        private readonly object _target;

        public CopyToVisitor(object target)
        {
            _target = target;
        }

        public void Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container)
        {
            foreach (var property in properties.GetProperties(ref container))
            {
                property.Accept(this, ref container);
            }
        }

        public void Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container)
        {
            var value = property.GetValue(ref container);
            PropertyContainer.SetValue(_target, property.Name, value);
            // if (_target is TContainer target)  // TODO: check if this is better performance-wise
            // {
            //     property.SetValue(ref target, value);
            // }
        }
    }
}