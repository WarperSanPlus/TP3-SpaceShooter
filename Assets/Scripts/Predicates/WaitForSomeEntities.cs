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

        #endregion

        [SerializeField]
        private BaseEntity[] entities;

        [SerializeField, Min(0)]
        private int minimum;

        public override bool GetCondition(float elapsed)
        {
            var amount = this.minimum;

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
