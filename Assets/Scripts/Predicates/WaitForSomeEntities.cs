using Entities;
using Interfaces;
using UnityEngine;

namespace Predicates
{
    internal class WaitForSomeEntities : Predicate, IPredicateEntity
    {
        #region IPredicateEntity

        public BaseEntity[] GetEntities() => this.entities;

        public void SetEntities(BaseEntity[] entities) => this.entities = entities;

        #endregion IPredicateEntity

        [SerializeField]
        private BaseEntity[] entities;

        [SerializeField, Min(0)]
        private int minimum;

        public override bool GetCondition(float elapsed)
            => IsValid(this.entities, this.minimum);

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
    }
}