﻿#if ENABLED_ATOMS
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    [CreateAssetMenu(menuName = "Facticus/Save System/Persistent variables list/Double variables",
        fileName = "PersistentDoubleVariables", order = 0)]
    public class PersistentDoubleVariables : BasePersistentVariablesList<double, DoublePair, DoubleConstant, DoubleVariable, DoubleEvent, DoublePairEvent, DoubleDoubleFunction, DoubleVariableInstancer, DoubleReference>
    {
    }
}
#endif