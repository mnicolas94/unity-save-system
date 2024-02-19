using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/SaveSystem/Persistent variables list/Bool variables",
        fileName = "PersistentBoolVariables", order = 0)]
    public class PersistentBoolVariables : BasePersistentVariablesList<bool, BoolPair, BoolEvent, BoolPairEvent, BoolBoolFunction>
    {
    }
}