using Emetters;
using UnityEngine;

namespace Serializables
{
    /// <summary>
    /// Defines a pattern of <see cref="Controllers.PatternController"/>
    /// </summary>
    [System.Serializable]
    public struct BulletPattern
    {
        [Min(0), Tooltip("Time before the attack starts")]
        public float startTime;

        [Min(0), Tooltip("Duration of the attack")]
        public float duration;

        [Min(0), Tooltip("Time after the attack ended")]
        public float exitTime;

        [Tooltip("Emetter used for this pattern")]
        public BaseEmetter emetter;
    }
}