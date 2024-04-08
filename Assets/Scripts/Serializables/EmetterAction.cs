using UnityEngine;

namespace Serializables
{
    /// <summary>
    /// Defines an action of an emetter
    /// </summary>
    [System.Serializable]
    public struct EmetterAction
    {
        [Tooltip("Script that will be modified upon trigger")]
        public MonoBehaviour script;

        [Tooltip("Determines if the script will be active or not")]
        public bool willBeActive;
    }
}