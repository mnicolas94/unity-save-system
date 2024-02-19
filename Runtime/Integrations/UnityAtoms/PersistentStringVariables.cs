#if ENABLED_ATOMS
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/Save System/Persistent variables list/String variables",
        fileName = "PersistentStringVariables", order = 0)]
    public class PersistentStringVariables : BasePersistentVariablesList<string, StringPair, StringConstant, StringVariable, StringEvent, StringPairEvent, StringStringFunction, StringVariableInstancer, StringReference>
    {
    }
}
#endif