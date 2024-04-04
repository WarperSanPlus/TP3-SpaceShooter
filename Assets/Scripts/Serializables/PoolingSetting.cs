using System;
using UnityEngine;

namespace Serializables
{
    /// <summary>
    /// Defines how this object pool should initializes
    /// </summary>
    [Serializable]
    public struct PoolingSetting
    {
        [Tooltip("GameObject to spawn")]
        public GameObject Prefab;

        [Min(0), Tooltip("Amount of objects to create at the start of this script")]
        public int amount;

        [Tooltip("Shows how many objects of this type has been spawned")]
        public int amountSpawned;

        /// <inheritdoc/>
        public override readonly string ToString() => $"\'{this.Prefab.name}\': {this.amount}";
    }
}