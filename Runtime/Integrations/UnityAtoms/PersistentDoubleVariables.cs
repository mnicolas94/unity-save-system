using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/SaveSystem/Persistent variables list/Double variables",
        fileName = "PersistentDoubleVariables", order = 0)]
    public class PersistentDoubleVariables : BasePersistentVariablesList<double, DoublePair, DoubleEvent, DoublePairEvent, DoubleDoubleFunction>
    {
    }
}