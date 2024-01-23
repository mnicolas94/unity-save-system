using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem.GuidsResolve
{
    public interface IGuidResolver
    {
        bool ExistsGuid(string guid);
        bool ExistsObject(Object obj);
        string GetGuid(Object obj);
        Object GetObject(string guid);
        bool TryGetGuid(Object obj, out string guid);
        bool TryGetObject(string guid, out Object obj);
        void PopulateDatabase(List<(Object, string)> references);
    }
}