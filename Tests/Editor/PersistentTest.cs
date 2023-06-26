using SaveSystem.Attributes;
using UnityEngine;

namespace SaveSystem.Tests.Editor
{
    [CreateAssetMenu(fileName = "PersistentTest", menuName = "Tests/PersistentTest")]
    [DoNotPersist(nameof(_float))]
    public class PersistentTest : ScriptableObject
    {
        [SerializeField] private int _int;
        [SerializeField, DoNotPersist] private string _string;
        [SerializeField] private float _float;

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

        public float F
        {
            get => _float;
            set => _float = value;
        }
    }
}