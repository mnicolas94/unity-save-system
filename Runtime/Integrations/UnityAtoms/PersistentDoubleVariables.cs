using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Persistence/Double variables", fileName = "PersistentDoubleVariables", order = 0)]
    public class PersistentDoubleVariables : BasePersistentVariablesList<double, DoublePair, DoubleEvent, DoublePairEvent, DoubleDoubleFunction>
    {
    }
}