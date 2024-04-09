using UnityEngine;

namespace Serializables
{
    /// <summary>
    /// Defines a prefab link between two objects
    /// </summary>
    [System.Serializable]
    public struct WaveLink
    {
        [Tooltip("ID of the source of this link")]
        public int From;

        [Tooltip("ID of the target of this link")]
        public int To;
    }
}