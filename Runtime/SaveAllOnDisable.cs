using UnityEngine;

namespace SaveSystem
{
    public class SaveAllOnDisable : MonoBehaviour
    {
        private void OnDisable()
        {
            SaveUtils.SaveAll();
        }
    }
}