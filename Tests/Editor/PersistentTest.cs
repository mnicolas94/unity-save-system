using SaveSystem.Attributes;
using UnityEngine;

namespace SaveSystem.Tests.Editor
{
    [CreateAssetMenu(fileName = "PersistentTest", menuName = "Tests/PersistentTest")]
    public class PersistentTest : ScriptableObject
    {
        [SerializeField] private int _int;
        [SerializeField, DoNotPersist] private string _string;

        public int I
        {
            get => _int;
            set => _int = value;
        }

        public string S
        {
            get => _string;
            set => _string = value;
        }
    }
}