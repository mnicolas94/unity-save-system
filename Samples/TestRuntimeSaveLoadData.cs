using UnityEngine;

namespace Samples
{
    [CreateAssetMenu(fileName = "TestRuntimeSaveLoadData", menuName = "Samples/TestRuntimeSaveLoadData", order = 0)]
    public class TestRuntimeSaveLoadData : ScriptableObject
    {
        public int IntField;
        public string StringField;
        public float FloatField;
        public bool BoolField;
        public Vector3 Position;
    
        public void Randomize()
        {
            IntField = (int)(Random.value * 100000);
            StringField = Random.value.ToString();
            FloatField = Random.value;
            BoolField = Random.value > 0.5f;
            Position = Random.insideUnitSphere;
        }
    }
}