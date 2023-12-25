using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace SaveSystem.Tests.Editor
{
    [Serializable]
    internal class SerializableClass
    {
        [SerializeField] private int _serializableField;
    }
    
    internal class NonSerializableClass
    {
        private int _nonSerializableField;
    }
    
    [Serializable]
    internal class Base
    {
        [SerializeField] private int _field1;
        [SerializeField] private string _field2;
        [SerializeField] private SerializableClass _serializableFieldObject;
        private NonSerializableClass _nonSerializableFieldObject;
    }

    [Serializable]
    internal class Child : Base
    {
        [SerializeField] private float _childField;
    }
    
    public class ReflectionTests
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void GetSerializableFieldsTest()
        {
            // arrange
            var type = typeof(Child);
            
            // act
            var fields = ReflectionUtils.GetSerializableFields(type);
            
            // assert
            var expected = new []
            {
                "_field1",
                "_field2",
                "_serializableFieldObject",
                "_childField",
            };
            
            Assert.IsTrue(fields.Count == expected.Length);
            
            foreach (var field in fields)
            {
                Assert.IsTrue(expected.Contains(field.Name));
            }
        }
    }
}