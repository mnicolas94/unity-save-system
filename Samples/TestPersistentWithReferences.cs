using System.Collections.Generic;
using UnityEngine;

namespace Samples
{
    [CreateAssetMenu(fileName = "TestPersistenceWithReferences", menuName = "TestPersistenceReferences", order = 0)]
    public class TestPersistentWithReferences : ScriptableObject
    {
        [SerializeField] private Object _reference1;
        [SerializeField] private Object _reference2;
        [SerializeField] private TestPersistent _reference3;
        [SerializeField] private List<Object> _list;
    }
}