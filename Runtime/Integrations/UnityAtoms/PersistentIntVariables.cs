using SaveSystem.Runtime.Integrations.UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Persistence/Int variable", fileName = "IntVariable", order = 0)]
    public class PersistentIntVariables : BasePersistentVariablesList<int, IntPair, IntEvent, IntPairEvent, IntIntFunction>
    {
    }
}