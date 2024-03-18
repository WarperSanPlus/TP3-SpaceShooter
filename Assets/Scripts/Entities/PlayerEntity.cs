using Controllers;
using Emetters;
using System;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Class that manages how the players behave
    /// </summary>
    public class PlayerEntity : BaseEntity
    {
        [Header("Player Entity")]
        [SerializeField, Tooltip("Player controller used")]
        private PlayerController controller;

        #region BaseEntity

        /// <inheritdoc/>
        protected override void OnHealthChanged(float newHealth, float oldHealth, float maxHealth)
        {
            var percentHealth = newHealth / maxHealth;

            var index = -1;

            // Find current emetter for health
            for (var i = 0; i < this.emetters.Length; i++)
            {
                EmetterForHealth item = this.emetters[i];
                var currentHealth = item.isPercent ? percentHealth : newHealth;

                if (item.health < currentHealth)
                    continue;

                index = i;
            }

            if (index != -1)
                this.controller.SetEmetter(this.emetters[index].emetter);
        }

        /// <inheritdoc/>
        protected override float OnDamageModifier(float amount, bool isAdding) => isAdding 
            ? base.OnDamageModifier(amount, isAdding)
            : amount * 5;

        #endregion BaseEntity

        #region EmetterForHealth

        [SerializeField, Tooltip("Emetters to use")]
        private EmetterForHealth[] emetters;

        [Serializable]
        public struct EmetterForHealth
        {
            [Tooltip("Emetter to use when reaching the target health")]
            public BaseEmetter emetter;

            [Tooltip("Amount of health needed to use this emetter")]
            public float health;

            [Tooltip("Determines if the health needed is a percent [0; 1] or a literal value")]
            public bool isPercent;
        }

        #endregion EmetterForHealth
    }
}
