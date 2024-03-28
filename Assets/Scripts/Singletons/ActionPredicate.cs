using Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Singletons
{
    public class ActionPredicate : Singleton<ActionPredicate>
    {
        [SerializeField]
        private List<PredicateRequest> predicates = new();

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion

        // Add a predicate
        // Remove a predicate
        // Clear all predicates

        public void Add(IPredicatable source, System.Func<float, bool> condition) 
            => this.predicates.Add(new PredicateRequest(source, condition));

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
                PredicateRequest predicate = this.predicates[i];

                // If the source exists and the condition is false, skip
                if (predicate.Exists() && !predicate.IsComplete(UnityEngine.Time.fixedDeltaTime))
                    continue;

                // Remove the predicate
                this.predicates.RemoveAt(i);

                // Trigger the source
                predicate.Trigger();
            }
        }

        /// <summary>
        /// Class that represents a request of validation
        /// </summary>
        [Serializable]
        private class PredicateRequest
        {
            /// <summary>
            /// The author of the request
            /// </summary>
            private readonly IPredicatable source;

            /// <summary>
            /// The condition of the request
            /// </summary>
            private readonly Func<float, bool> condition;

            public PredicateRequest(IPredicatable source) : this(source, null) { }

            public PredicateRequest(IPredicatable source, Func<float, bool> condition)
            {
                this.source = source;
                this.condition = condition;
            }

            /// <summary>
            /// Calls <see cref="IPredicatable.Trigger"/> of the author
            /// </summary>
            public void Trigger() => this.source?.Trigger();

            /// <returns>Is <paramref name="source"/> the author of this request?</returns>
            public bool IsSource(IPredicatable source) => this.source == source;

            /// <returns>Does the source still exists?</returns>
            public bool Exists() => this.source != null;

            /// <returns>Has the request achieved is validation point?</returns>
            public bool IsComplete(float elapsed) => this.condition?.Invoke(elapsed) ?? true;
        }
    }
}