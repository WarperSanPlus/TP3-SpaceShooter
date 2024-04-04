using System;
using UnityEngine;

namespace Serializables
{
    /// <summary>
    /// Defines the category under which the settings are
    /// </summary>
    [Serializable]
    public struct PoolingType
    {
        [Tooltip("Under which category those objects are")]
        public string Namespace;

        [Tooltip("List of settings associated with this type")]
        public PoolingSetting[] Settings;
    }
}