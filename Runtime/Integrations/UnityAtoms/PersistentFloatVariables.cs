using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Persistence/Float variables", fileName = "PersistentFloatVariables", order = 0)]
    public class PersistentFloatVariables : BasePersistentVariablesList<float, FloatPair, FloatEvent, FloatPairEvent, FloatFloatFunction>
    {
    }
}