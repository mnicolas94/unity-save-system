using System;
using System.Globalization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.GuidsResolve
{
    /// <summary>
    /// A serializable version of <see cref="GlobalObjectId"/>
    /// </summary>
    [Serializable]
    public class SerializableGlobalObjectId : IEquatable<SerializableGlobalObjectId>
    {
        [SerializeField] private ulong _guidLow;
        [SerializeField] private ulong _guidHigh;
        [SerializeField] private ulong _targetObjectId;
        [SerializeField] private ulong _targetPrefabId;
        [SerializeField] private int _identifierType;

        public SerializableGlobalObjectId()
        {
            
        }

#if UNITY_EDITOR
        public SerializableGlobalObjectId(Object reference)
        {
            ObjectReference = reference;
        }

        private Object _cachedReference;
        public Object ObjectReference
        {
            get
            {
                if (null != _cachedReference)
                {
                    return _cachedReference;
                }
                return GlobalObjectId.GlobalObjectIdentifierToObjectSlow(ToGlobalObjectId());
            }

            set
            {
                var globalId = GlobalObjectId.GetGlobalObjectIdSlow(value);
                _identifierType = globalId.identifierType;
                _targetObjectId = globalId.targetObjectId;
                _targetPrefabId = globalId.targetPrefabId;
                TryParseGuid(globalId.assetGUID.ToString(), out _guidLow, out _guidHigh);
                _cachedReference = value;
            }
        }

        private GUID GetGuid()
        {
            return new GUID(GetGuidString());
        }

        private string ToGlobalObjectIdString()
        {
            return string.Concat(
                "GlobalObjectId_V1-", _identifierType.ToString(),
                "-", GetGuidString(),
                "-", _targetObjectId.ToString(),
                "-", _targetPrefabId.ToString()
                );
        }
        
        private GlobalObjectId ToGlobalObjectId()
        {
            GlobalObjectId.TryParse(ToGlobalObjectIdString(), out var globalId);
            return globalId;
        }
#endif
        
        private bool TryParseGuid(string guidString, out ulong low, out ulong high)
        {
            low = 0;
            high = 0;
            if (guidString.Length != 32)
                return false;

            var parsedLow = ulong.TryParse(guidString.AsSpan(0, 16), NumberStyles.HexNumber, null, out low);
            var parsedHigh = ulong.TryParse(guidString.AsSpan(16, 16), NumberStyles.HexNumber, null, out high);

            return parsedLow && parsedHigh;
        }

        public string GetGuidString()
        {
            return string.Concat(_guidLow.ToString("x16"), _guidHigh.ToString("x16"));
        }

        public override string ToString()
        {
            return string.Concat(_identifierType.ToString(), GetGuidString(), _targetObjectId.ToString(), _targetPrefabId.ToString());
        }

        public bool Equals(SerializableGlobalObjectId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _guidLow == other._guidLow && _guidHigh == other._guidHigh && _targetObjectId == other._targetObjectId
                   && _targetPrefabId == other._targetPrefabId && _identifierType == other._identifierType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SerializableGlobalObjectId)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_guidLow, _guidHigh, _targetObjectId, _targetPrefabId, _identifierType);
        }
    }
}