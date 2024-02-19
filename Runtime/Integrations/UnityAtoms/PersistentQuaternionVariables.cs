using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/SaveSystem/Persistent variables list/Quaternion variables",
        fileName = "PersistentQuaternionVariables", order = 0)]
    public class PersistentQuaternionVariables : BasePersistentVariablesList<Quaternion, QuaternionPair, QuaternionEvent, QuaternionPairEvent, QuaternionQuaternionFunction>
    {
    }
}