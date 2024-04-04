using UnityEngine;

namespace Predicates
{
    /// <summary>
    /// Waits for the given amount of time
    /// </summary>
    public class WaitForSeconds : Predicate
    {
        [SerializeField, Min(0), Tooltip("Amount of time in seconds before the condition becomes true")]
        private float time = 0;

        /// <inheritdoc/>
        public override bool GetCondition(float elapsed)
        {
            this.time -= elapsed;

            return this.time <= 0;
        }
    }
}