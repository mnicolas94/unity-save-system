using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Persistence/String variables", fileName = "PersistentStringVariables", order = 0)]
    public class PersistentStringVariables : BasePersistentVariablesList<string, StringPair, StringEvent, StringPairEvent, StringStringFunction>
    {
    }
}