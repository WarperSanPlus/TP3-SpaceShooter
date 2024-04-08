using Entities;
using Interfaces;
using UnityEngine;

namespace Predicates
{
    /// <summary>
    /// Waits until at least the minimum amount of entities is disabled
    /// </summary>
    internal class WaitForSomeEntities : Predicate, IPredicateEntity
    {
        [SerializeField, Tooltip("List of entities to check")]
        private BaseEntity[] entities;

        [SerializeField, Min(0), Tooltip("Minimum amount of entities needed to activate the predicate")]
        private int minimum;

        /// <param name="entities">List of entities to check</param>
        /// <param name="minimum">Minimum amount of entities needed to activate the predicate</param>
        /// <returns>Is the predicate complete?</returns>
        public static bool IsValid(BaseEntity[] entities, int minimum)
        {
            var amount = minimum;

            foreach (BaseEntity item in entities)
            {
                if (item != null && item.gameObject.activeInHierarchy)
                    continue;

                amount--;

                if (amount <= 0)
                    return true;
            }

            return false;
        }

        #region Predicate

        /// <inheritdoc/>
        public override bool GetCondition(float elapsed) => IsValid(this.entities, this.minimum);

        #endregion

        #region IPredicateEntity

        /// <inheritdoc/>
        public BaseEntity[] GetEntities() => this.entities;

        /// <inheritdoc/>
        public void SetEntities(BaseEntity[] entities) => this.entities = entities;

        #endregion IPredicateEntity
    }
}