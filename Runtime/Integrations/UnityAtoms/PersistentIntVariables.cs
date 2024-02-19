#if ENABLED_ATOMS
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/Save System/Persistent variables list/Int variable",
        fileName = "IntVariable", order = 0)]
    public class PersistentIntVariables : BasePersistentVariablesList<int, IntPair, IntConstant, IntVariable, IntEvent, IntPairEvent, IntIntFunction, IntVariableInstancer, IntReference>
    {
    }
}
#endif