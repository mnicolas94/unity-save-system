using System;
using System.Linq;
using NUnit.Framework;
using SaveSystem.Attributes;
using UnityEngine;

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
    
    [OnlyPersist("_field1", "_field2")]
    [Serializable]
    internal class ChildOnlyFields : Base
    {
        [SerializeField] private float _childField;
    }
    
    [PersistDeclaredOnly]
    [Serializable]
    internal class ChildDeclared : Base
    {
        [SerializeField] private float _only;
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
        
        [Test]
        public void GetSerializableFieldsWithPersistOnlyAttributeTest()
        {
            // arrange
            var type = typeof(ChildOnlyFields);
            
            // act
            var fields = ReflectionUtils.GetSerializableFields(type);
            
            // assert
            var expected = new []
            {
                "_field1",
                "_field2",
            };
            
            Assert.IsTrue(fields.Count == expected.Length);
            
            foreach (var field in fields)
            {
                Assert.IsTrue(expected.Contains(field.Name));
            }
        }
        
        [Test]
        public void GetSerializableFieldsWithPersistDeclaredOnlyAttributeTest()
        {
            // arrange
            var type = typeof(ChildDeclared);
            
            // act
            var fields = ReflectionUtils.GetSerializableFields(type);
            
            // assert
            var expected = new []
            {
                "_only",
            };
            
            Assert.IsTrue(fields.Count == expected.Length);
            
            foreach (var field in fields)
            {
                Assert.IsTrue(expected.Contains(field.Name));
            }
        }
    }
}