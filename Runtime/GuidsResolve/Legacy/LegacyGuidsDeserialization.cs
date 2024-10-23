using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SaveSystem.GuidsResolve.Legacy
{
    public static class LegacyGuidsDeserialization
    {
        private const string Separator = "---";
        private const string NewIdsPattern = @"GlobalObjectId_V1-\d+-([a-f\d]+)-(\d+)-(\d+)";

        public static bool TryGetObjectFromOldGuid(string guid, Dictionary<string, Object> idToAsset, out Object obj)
        {
            obj = null;
            var newIds = idToAsset.Keys.ToList();

            
            if (guid.Length == 32 && TryToGetNewIdFromGuid(guid, newIds, out var newId))  // it is a guid
            {
                obj = idToAsset[newId];
                return true;
            }

            if (!guid.Contains(Separator)) return false;
            
            // old way of serializing sub-assets and components inside prefabs
            var splits = guid.Split(Separator);
            if (splits.Length != 2) return false;
            
            var guidPart = splits[0];
            var localIdPart = splits[1];
            if (!TryToGetNewIdFromGuidAndLocalId(guidPart, localIdPart, newIds, out newId)) return false;
            
            obj = idToAsset[newId];
            return true;
        }

        private static bool TryToGetNewIdFromGuid(string guid, List<string> newIds, out string newId)
        {
            var regex = new Regex(NewIdsPattern);
            foreach (var id in newIds)
            {
                var match = regex.Match(id);
                if (match.Success && match.Groups[1].Value == guid)
                {
                    newId = id;
                    return true;
                }
            }

            newId = string.Empty;
            return false;
        }
        
        private static bool TryToGetNewIdFromGuidAndLocalId(string guid, string localId, List<string> newIds, out string newId)
        {
            newId = string.Empty;
            var regex = new Regex(NewIdsPattern);

            // we need to convert the localId to an unsigned long.
            // More info at https://docs.unity3d.com/ScriptReference/GlobalObjectId.html#:~:text=Note%3A%20Actual%20local,find%20an%20object. 
            if (!long.TryParse(localId, out var localIdLong)) return false;
            var localIdUlong = (ulong)localIdLong;
            var convertedLocalId = localIdUlong.ToString();
            
            foreach (var id in newIds)
            {
                var match = regex.Match(id);
                if (match.Success && match.Groups[1].Value == guid && match.Groups[2].Value == convertedLocalId)
                {
                    newId = id;
                    return true;
                }
            }

            return false;
        }
    }
}