using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Utilities
{
    public abstract class IdReference
    {
        public const string IdFieldName = nameof(_id);
      
        [SerializeField] private string _id;
        
        private Object _cachedObject;
        private string _cachedId;
        public Object Object
        {
            get
            {
                if (_cachedId != _id)
                {
                    var resolver = SaveSystemSettings.Instance.GuidsResolver;
                    if (resolver.TryGetObject(_id, out _cachedObject))
                    {
                        _cachedId = _id;
                    }
                }

                return _cachedObject;
            }
            set
            {
                if (_cachedId == _id && _cachedObject == value) return;
                
                var resolver = SaveSystemSettings.Instance.GuidsResolver;
                if (resolver.TryGetGuid(value, out var id))
                {
                    _id = id;
                    _cachedId = id;
                    _cachedObject = value;
                }
            }
        }
        
        internal abstract Type GetReferencedType();
    }
    
    /// <summary>
    /// An object that can accept a value of type T in the inspector, but will be serialized as a unique Id instead
    /// of the actual object. The object can be accessed with the <see cref="get_Object"/> property.
    /// It uses the save system's reference resolver (see <see cref="IGuidResolver"/>) to retrieve the objects and ids.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IdReference<T> : IdReference where T : Object
    {
        public new T Object
        {
            get => base.Object as T;
            set => base.Object = value;
        }
        
        public static implicit operator T(IdReference<T> sId)
        {
            return sId.Object;
        }

        internal override Type GetReferencedType()
        {
            return typeof(T);
        }
    }
}