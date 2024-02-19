#if ENABLED_ATOMS
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/Save System/Persistent variables list/Vector2 variables",
        fileName = "PersistentVector2Variables", order = 0)]
    public class PersistentVector2Variables : BasePersistentVariablesList<Vector2, Vector2Pair, Vector2Event, Vector2PairEvent, Vector2Vector2Function>
    {
    }
}
#endif