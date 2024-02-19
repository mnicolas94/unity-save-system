#if ENABLED_ATOMS
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/SaveSystem/Persistent variables list/Color variables",
        fileName = "PersistentColorVariables", order = 0)]
    public class PersistentColorVariables : BasePersistentVariablesList<Color, ColorPair, ColorEvent, ColorPairEvent, ColorColorFunction>
    {
    }
}
#endif