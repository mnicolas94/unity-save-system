using UnityEngine;

namespace SaveSystem.Tests.Editor
{
    public class TestsReferences : ScriptableObject
    {
        [SerializeField] public PersistentTest persistent;
        [SerializeField] public GameObject prefab;

        private static TestsReferences _instance;
        public static TestsReferences Instance => _instance ??= CreateInstance<TestsReferences>();
    }
}