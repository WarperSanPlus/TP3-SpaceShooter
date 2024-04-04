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

        #endregion Singleton

        #region MonoBehaviour

        /// <inheritdoc/>
        private void FixedUpdate()
        {
            this.RemoveCompletedPredicates();
            this.UpdatePredicates(Time.fixedDeltaTime);
        }

        #endregion MonoBehaviour

        #region Update

        private readonly List<Guid> completedPredicates = new();

        private void RemoveCompletedPredicates()
        {
            // Remove predicates
            for (var i = 0; i < this.completedPredicates.Count; i++)
            {
                Guid guid = this.completedPredicates[i];
                var index = this.predicates.FindIndex(p => p.guid == guid);

                if (index == -1)
                    continue;

                this.predicates.RemoveAt(index);
            }

            this.completedPredicates.Clear();
        }

        private void UpdatePredicates(float elapsed)
        {
            // Check every predicates
            for (var i = this.predicates.Count - 1; i >= 0; i--)
            {
                if (i >= this.predicates.Count)
                    continue;

                PredicateRequest predicate = this.predicates[i];

                // If the source exists and the condition is false, skip
                if (predicate.Exists() && !predicate.IsComplete(elapsed))
                    continue;

                // Remove the predicate
                this.completedPredicates.Add(predicate.guid);

                // Trigger the source
                predicate.Trigger();
            }
        }

        #endregion Update

        #region Add

        /// <summary>
        /// Creates a predicate for the given condition
        /// </summary>
        /// <returns>Unique ID of the request</returns>
        public Guid Add(IPredicatable source, Func<float, bool> condition)
        {
            var request = new PredicateRequest(source, condition);
            this.predicates.Add(request);

            return request.guid;
        }

        #endregion Add

        #region Remove

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
                this.completedPredicates.Add(this.predicates[i].guid);

                // Decrease amount by 1
                limit--;

                // If limit reached, break
                if (limit == 0)
                    break;
            }
        }

        public void Remove(Guid guid)
        {
            for (var i = this.predicates.Count - 1; i >= 0; i--)
            {
                // Skip if not from given source
                if (this.predicates[i].guid != guid)
                    continue;

                // Remove
                this.completedPredicates.Add(this.predicates[i].guid);
                break;
            }
        }

        #endregion Remove

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

            public readonly Guid guid;

            public PredicateRequest(IPredicatable source) : this(source, null)
            {
            }

            public PredicateRequest(IPredicatable source, Func<float, bool> condition)
            {
                this.source = source;
                this.condition = condition;
                this.guid = Guid.NewGuid();
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