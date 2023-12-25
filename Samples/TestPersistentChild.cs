using System;
using UnityEngine;

namespace Samples
{
    public class TestPersistentBase : ScriptableObject
    {
        [SerializeField] private int _baseInt;
        [SerializeField] private float _baseFloat;
    }
    
    [CreateAssetMenu(fileName = "TestPersistentChild", menuName = "TestPersistentChild", order = 0)]
    public class TestPersistentChild : TestPersistentBase
    {
        [SerializeField] private string _string;
    }
}