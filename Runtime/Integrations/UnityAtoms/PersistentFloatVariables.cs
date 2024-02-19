using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/SaveSystem/Persistent variables list/Float variables",
        fileName = "PersistentFloatVariables", order = 0)]
    public class PersistentFloatVariables : BasePersistentVariablesList<float, FloatPair, FloatEvent, FloatPairEvent, FloatFloatFunction>
    {
    }
}