using Interfaces;
using Predicates;
using System.Collections.Generic;
using UnityEngine;

namespace Singletons
{
    public class ActionPredicate : Singleton<ActionPredicate>
    {
        [SerializeField]
        private List<Predicate> predicates = new();

        // Add a predicate
        // Remove a predicate
        // Clear all predicates

        public void Add(IPredicatable source, System.Func<float, bool> condition) 
            => this.predicates.Add(new Predicate(source, condition));

        public void Remove(IPredicatable source, int limit = -1)
        {
            // Set to max value
            if (limit < 0)
                limit = int.MaxValue;

            for (var i = this.predicates.Count - 1; i >= 0; i--)
            {
                // Skip if not from given source
                if (!this.predicates[i].IsSource(source))
                    continue;

                // Remove
                this.predicates.RemoveAt(i);

                // Decrease amount by 1
                limit--;

                // If limit reached, break
                if (limit == 0)
                    break;
            }
        }

        private void FixedUpdate()
        {
            for (var i = this.predicates.Count - 1; i >= 0; i--)
            {
                Predicate predicate = this.predicates[i];

                // If the source exists and the condition is false, skip
                if (predicate.Exists() && !predicate.IsComplete(UnityEngine.Time.fixedDeltaTime))
                    continue;

                // Remove the predicate
                this.predicates.RemoveAt(i);

                // Trigger the source
                predicate.Trigger();
            }
        }
    }
}