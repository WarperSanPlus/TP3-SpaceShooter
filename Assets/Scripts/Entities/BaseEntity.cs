using Interfaces;
using System.Collections;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Class that manages how the entities behave
    /// </summary>
    public class BaseEntity : MonoBehaviour, IActivatable, IResetable
    {
        public const string TAG_BULLET = "Bullet";
        public const string TAG_ENEMY = "Enemy";

        #region MonoBehaviour

        /// <inheritdoc/>
        private void FixedUpdate()
        {
            if (this.iframes != 0)
                this.iframes--;

            this.OnUpdate(Time.fixedDeltaTime);
        }

        /// <inheritdoc/>
        private void OnTriggerEnter2D(Collider2D collision) => this.ManageCollision(collision);

        /// <inheritdoc/>
        private void OnTriggerStay2D(Collider2D collision) => this.ManageCollision(collision);

        /// <inheritdoc/>
        private void OnDisable()
        {
            if (this.hitBlinkCoroutine != null)
                this.StopCoroutine(this.hitBlinkCoroutine);
        }

        #endregion MonoBehaviour

        #region IActivatable

        /// <inheritdoc/>
        public void SetActive(bool isActive) => this.isInvicible = !isActive;

        #endregion

        #region IResetable

        /// <inheritdoc/>
        public virtual void OnReset()
        {
            this.iframes = 0;
            this.health = this.MaxHealth;
            this.hitBlinkSprite.color = Color.white;
            this.hitBlinkCoroutine = null;
        }

        #endregion

        #region I-frames

        [SerializeField, Min(0)]
        [Tooltip("Number of seconds where the entity cannot be damaged after a hit")]
        private float invincibilityTime = 0;
        private int iframes = 0;

        #endregion I-frames

        #region Health

        [SerializeField, Tooltip("Current health of the entity")]
        private float health;

        [SerializeField, Tooltip("Maximum health reachable by the entity")]
        protected float MaxHealth;

        [Tooltip("Determines if this entity can take damage")]
        public bool isInvicible = true;

        private void ManageCollision(Collider2D collider)
        {
            if (!this.ShouldCollide(collider))
                return;

            // Depending on the tag, execute a different action
            if (collider.gameObject.CompareTag(TAG_BULLET) || collider.gameObject.CompareTag(TAG_ENEMY))
            {
                this.Damage(1);
            }

            // Collect the item
            ICollectable collectable = collider.GetComponentInParent<ICollectable>();
            collectable?.Collect(this);
        }

        public void Health(int amount) => this.AlterHealth(amount, true);

        public void Damage(float amount)
        {
            // If the entity is on invincibility
            if (this.iframes != 0 || !this.enabled || this.isInvicible)
                return;

            this.AlterHealth(amount, false);
        }

        private void AlterHealth(float amount, bool isAdding)
        {
            // If the entity is dead
            if (this.health <= 0f)
                return;

            // Remember the previous health
            var oldHealth = this.health;

            // Calculate the damage amount
            amount = this.OnDamageModifier(amount, isAdding);

            // Don't do anything if no damage
            if (amount == 0)
                return;

            // Modify health
            this.health += isAdding ? amount : -amount;
            this.health = Mathf.Clamp(this.health, 0, this.MaxHealth);

            this.OnHealthChanged(
                this.health,
                oldHealth,
                this.MaxHealth
                );

            // If the entity will die
            if (this.health <= 0f)
                return;

            if (!isAdding)
            {
                this.iframes = Mathf.FloorToInt(this.invincibilityTime / Time.fixedDeltaTime);
                this.TriggerHitBlink();
            }
        }

        #endregion Health

        #region Hit Blink

        private const float HIT_BLINK_INTERVAL = 0.05f;

        [SerializeField, Tooltip("Sprite to apply the blink effect on")]
        private SpriteRenderer hitBlinkSprite;

        [SerializeField, Tooltip("Color of the blink effect")]
        private Color hitBlinkColor;

        private Coroutine hitBlinkCoroutine = null;

        private void TriggerHitBlink() => this.hitBlinkCoroutine ??= this.StartCoroutine(this.FlashSprite());

        // Taken from: https://forum.unity.com/threads/make-a-sprite-flash.224086/#post-1837621
        private IEnumerator FlashSprite()
        {
            Color originalColor = this.hitBlinkSprite.color;

            Color[] colors = { originalColor, this.hitBlinkColor };

            var index = 0;
            while (this.iframes > 0)
            {
                this.hitBlinkSprite.color = colors[index % 2];

                index++;
                yield return new WaitForSeconds(HIT_BLINK_INTERVAL);
            }

            this.hitBlinkSprite.color = originalColor;

            // Clear coroutine
            this.hitBlinkCoroutine = null;
        }

        #endregion Hit Blink

        #region Virtual

        /// <summary>
        /// Called when the entity's health changed
        /// </summary>
        /// <param name="newHealth">New value of <see cref="health"/></param>
        /// <param name="oldHealth">Previous value of <see cref="health"/></param>
        /// <param name="maxHealth">Value of <see cref="MaxHealth"/></param>
        protected virtual void OnHealthChanged(float newHealth, float oldHealth, float maxHealth) { }

        /// <param name="amount">Current amount of damage</param>
        /// <returns>How much damage does the attack do?</returns>
        protected virtual float OnDamageModifier(float amount, bool isAdding) => amount;

        /// <summary>
        /// Called when <see cref="FixedUpdate"/> is called
        /// </summary>
        /// <param name="elapsed">Time elapsed since the last call</param>
        protected virtual void OnUpdate(float elapsed) { }

        /// <returns>Should the collision be managed?</returns>
        protected virtual bool ShouldCollide(Collider2D collision) => true;

        #endregion Virtual
    }
}