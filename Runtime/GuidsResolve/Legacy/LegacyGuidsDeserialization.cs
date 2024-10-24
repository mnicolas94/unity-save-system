using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SaveSystem.GuidsResolve.Legacy
{
    public static class LegacyGuidsDeserialization
    {
        private const string Separator = "---";
        private const string NewIdsPattern = @"GlobalObjectId_V1-\d+-([a-f\d]+)-(\d+)-(\d+)";
        private const string ScriptaObjectsMainLocalId = "11400000";

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
        
        public static bool TryGetOldGuidFromNewOne(string newId, out string guid)
        {
            var regex = new Regex(NewIdsPattern);
            var match = regex.Match(newId);
            if (match.Success)
            {
                var guidPart = match.Groups[1].Value;
                var localIdPart = match.Groups[2].Value;
                
                guid = guidPart;

                if (localIdPart != ScriptaObjectsMainLocalId)
                {
                    // we need to convert the localIdPart to a signed long.
                    // More info at https://docs.unity3d.com/ScriptReference/GlobalObjectId.html#:~:text=Note%3A%20Actual%20local,find%20an%20object. 
                    var localId = (long)ulong.Parse(localIdPart);
                    var localIdString = localId.ToString();
                    guid = string.Concat(guid, Separator, localIdString);
                }

                return true;
            }

            guid = String.Empty;
            return false;
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