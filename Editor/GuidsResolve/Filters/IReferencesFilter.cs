using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem.Editor.GuidsResolve.Filters
{
    public interface IReferencesFilter
    {
        void AddObjectsAndGuids(List<(Object, string)> objectsGuids);
    }
}