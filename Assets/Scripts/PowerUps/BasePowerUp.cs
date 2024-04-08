using Entities;
using Interfaces;
using UnityEngine;

namespace PowerUps
{
    public class BasePowerUp : MonoBehaviour, ILifespan, ICollectable
    {
        public const string NAMESPACE = "PowerUps";

        [SerializeField, Min(0), Tooltip("Amount of health that this power recovers")]
        private int healAmount = 25;

        #region ICollectable

        /// <inheritdoc/>
        public void Collect(BaseEntity source)
        {
            source.Heal(this.healAmount);

            this.gameObject.SetActive(false);
        }

        #endregion ICollectable

        #region ILifespan

        /// <inheritdoc/>
        public void OnLifeEnd() => this.gameObject.SetActive(false);

        #endregion ILifespan
    }
}