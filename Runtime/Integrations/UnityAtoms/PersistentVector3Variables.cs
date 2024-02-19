#if ENABLED_ATOMS
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/Save System/Persistent variables list/Vector3 variables",
        fileName = "PersistentVector3Variables", order = 0)]
    public class PersistentVector3Variables : BasePersistentVariablesList<Vector3, Vector3Pair, Vector3Event, Vector3PairEvent, Vector3Vector3Function>
    {
    }
}
#endif