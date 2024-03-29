﻿#if ENABLED_ATOMS
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/Save System/Persistent variables list/Bool variables",
        fileName = "PersistentBoolVariables", order = 0)]
    public class PersistentBoolVariables : BasePersistentVariablesList<bool, BoolPair, BoolConstant, BoolVariable, BoolEvent, BoolPairEvent, BoolBoolFunction, BoolVariableInstancer, BoolReference>
    {
    }
}
#endif