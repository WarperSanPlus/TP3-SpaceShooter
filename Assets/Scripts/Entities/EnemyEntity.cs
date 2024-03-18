using Controllers;
using Interfaces;
using Singletons;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Class that manages how the enemies behave
    /// </summary>
    public class EnemyEntity : BaseEntity, ILifespan
    {
        [Header("Enemy Entity")]
        [SerializeField, Tooltip("Determines if, on death, its' bullets will be destroyed")]
        private bool destroyBulletsOnDeath = false;

        private void KillSelf(bool fromPlayer)
        {
            if (fromPlayer)
            {
                // Explode
                GameObject obj = ObjectPool.Instance.GetRandomObject("Explosions");
                obj.transform.position = this.transform.position;
                obj.SetActive(true);

                if (this.destroyBulletsOnDeath)
                {
                    foreach (BaseController item in this.GetComponents<BaseController>())
                        item.DestroyBullets();
                }

                GameObject powerUp = ObjectPool.Instance.GetRandomObject(PowerUps.BasePowerUp.NAMESPACE);
                powerUp.transform.position = this.transform.position;
                powerUp.SetActive(true);

                this.PlaySound(this.deathSound);
            }

            // Destroy enemy
            this.gameObject.SetActive(false);
        }

        #region BaseEntity

        /// <inheritdoc/>
        protected override bool ShouldCollide(Collider2D collision) => collision.gameObject.CompareTag(TAG_BULLET);

        /// <inheritdoc/>
        protected override void OnHealthChanged(float newHealth, float oldHealth, float maxHealth)
        {
            if (newHealth > 0f)
            {
                this.PlaySound(this.hitSound);
                return;
            }

            this.KillSelf(true);
        }

        #endregion BaseEntity

        #region SFX

        [SerializeField, Tooltip("Prefab used to play the hit sound")]
        private GameObject hitSound;

        [SerializeField, Tooltip("Prefab used to play the death sound")]
        private GameObject deathSound;

        private void PlaySound(GameObject sound)
        {
            if (sound == null)
                return;

            ObjectPool.Instance.GetPooledObject(sound.name, SFX_Object.NAMESPACE);
        }

        #endregion SFX

        #region ILifespan

        /// <inheritdoc/>
        public void OnLifeEnd() => this.KillSelf(false);

        #endregion ILifespan
    }
}