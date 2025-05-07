using UnityEngine;

namespace SaveSystem.Utilities
{
    public static class Logging
    {
        public static void DebugLog(string message)
        {
            if (SaveSystemSettings.Instance.DebugLogging)
            {
                Debug.Log(message);
            }
        }

        public static void DebugLogError(string message)
        {
            // always log errors
            Debug.LogError(message);
        }
    }
}