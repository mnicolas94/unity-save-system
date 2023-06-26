using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Samples
{
    [Serializable]
    public class A
    {
        [SerializeField] private string _a;
    }
    
    [Serializable]
    public class B : A
    {
        [SerializeField] private string _b;
    }
    
    [Serializable]
    public class C : B
    {
        [SerializeField] private string _c;
    }
    
    [Serializable]
    public class D : C
    {
        [SerializeField] private string _d;
    }
    
    [Serializable]
    public class E : D
    {
        [SerializeField] private string _e;
    }
    
    public class TestRuntimeFields : MonoBehaviour
    {
        [ContextMenu("Print rf")]
        public void PrintRF()
        {
            var objects = new object[]
            {
                new A(),
                new B(),
                new C(),
                new D(),
                new E()
            };

            foreach (var obj in objects)
            {
                var type = obj.GetType();
                Debug.Log(type);
                var runtimeFieldsNames = type.GetRuntimeFields().Select(f => f.Name);
                Debug.Log(string.Join(", ", runtimeFieldsNames));
                var fieldsNames = type.GetFields().Select(f => f.Name);
                Debug.Log(string.Join(", ", fieldsNames));
            }

            var aType = typeof(A);
            var bType = typeof(B);
            Debug.Log($"a is b: {aType.IsAssignableFrom(bType)}");
            Debug.Log($"b is a: {bType.IsAssignableFrom(aType)}");
        }
    }
}