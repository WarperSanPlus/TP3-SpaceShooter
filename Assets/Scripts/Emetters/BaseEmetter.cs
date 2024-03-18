using Bullets;
using Extensions;
using Interfaces;
using Serializables;
using Singletons;
using System;
using UnityEngine;

namespace Emetters
{
    /// <summary>
    /// Class that manages how the emetters behave
    /// </summary>
    public abstract class BaseEmetter : MonoBehaviour, IActivatable
    {
        /// <summary>
        /// Initializes this emetter
        /// </summary>
        /// <param name="targetLayer">Layer where the projectiles will be</param>
        /// <param name="cooldown">Delay in seconds between every shoot</param>
        public virtual void Init(int targetLayer, Guid author, float cooldown = -1)
        {
            this.gameObject.layer = targetLayer;
            this.author = author;
            this.cooldown = Mathf.Max(cooldown, this.cooldown);
        }

        #region IActivatable

        /// <inheritdoc/>
        public void SetActive(bool isActive) => this.enabled = isActive;

        #endregion

        #region Author

        /// <summary>
        /// Unique identifiant of its controller
        /// </summary>
        private Guid author;

        #endregion Author

        #region Timer

        /// <summary>
        /// Internal timer used by this emetter
        /// </summary>
        private float timer = -1f;

        /// <summary>
        /// Advances the timer
        /// </summary>
        /// <param name="elapsed">Time in seconds since the last call</param>
        /// <param name="shootOnTimerEnd">If the timer reaches zero, should the emetter shoot</param>
        /// <returns>This call caused the emetter to fire</returns>
        public virtual bool Tick(float elapsed, bool shootOnTimerEnd = true)
        {
            // Reduce timer
            this.timer -= elapsed;

            // If the emetter can't shoot, skip
            if (this.timer > 0f || !shootOnTimerEnd)
                return false;

            this.Fire();
            
            // Reset the timer
            this.timer = this.cooldown;

            return true;
        }

        #endregion Timer

        #region Fire

        [SerializeField, Tooltip("Prefab of the projectile used by this emetter")]
        private GameObject bulletPrefab;

        [SerializeField, Min(0), Tooltip("Delay in seconds between every shoot")]
        protected float cooldown = 0;

        /// <summary>
        /// Triggers one round of this emetter
        /// </summary>
        private void Fire()
        {
            var amount = this.GetProjectileCount();

            for (var i = 0; i < amount; i++)
                this.ShootProjectile(i);
        }

        /// <summary>
        /// Shoots a projectile with the parameters from the methods
        /// </summary>
        /// <param name="index">Index of the projectile</param>
        private void ShootProjectile(int index)
        {
            // Get projectile
            GameObject currentProjectile = this.GetProjectile(index);

            // Skip if projectile is null
            if (currentProjectile == null)
            {
                Debug.LogWarning("Tried to get a projectile, but no projectile was defined.");
                return;
            }

            // Set up projectile
            currentProjectile.transform.SetPositionAndRotation(this.GetOrigin(index), this.GetRotation(index));

            // Set up bullet
            if (currentProjectile.TryGetComponent(out BaseBullet bullet))
            {
                bullet.SetLayer(this.gameObject.layer);
                bullet.Author = this.author;
            }
            else
            {
                Debug.LogWarning($"\'{currentProjectile.name}\' was created without a BaseBullet script.");
                currentProjectile.SetLayerRecursive(this.gameObject.layer);
            }

            // Reset every IResetable
            foreach (IResetable item in currentProjectile.GetComponents<IResetable>())
                item.OnReset();

            // Call every IActivatable
            foreach (IActivatable item in currentProjectile.GetComponents<IActivatable>())
                item.SetActive(true);

            currentProjectile.SetActive(true);
        }

        #endregion Fire

        #region Actions

        [Header("Actions")]
        [SerializeField, Tooltip("Actions called when this emetter starts being used")]
        private EmetterAction[] startActions = null;

        [SerializeField, Tooltip("Actions called when this emetter stops being used")]
        private EmetterAction[] endActions = null;

        /// <summary>
        /// Calls every actions that are associated with the start
        /// </summary>
        public virtual void OnStart() => this.startActions.CallActions();

        /// <summary>
        /// Calls every actions that are associated with the end
        /// </summary>
        public virtual void OnEnd() => this.endActions.CallActions();

        #endregion Actions

        #region Parameters

        /// <returns>Amount of projectiles to spawn on this call</returns>
        protected virtual int GetProjectileCount() => 1;

        /// <returns><see cref="GameObject"/> that will be used for the current projectile at <paramref name="index"/></returns>
        protected virtual GameObject GetProjectile(int index) => this.bulletPrefab == null
            ? null
            : ObjectPool.Instance.GetPooledObject(this.bulletPrefab.name, BaseBullet.NAMESPACE);

        /// <returns>Starting position of the projectile at <paramref name="index"/></returns>
        protected virtual Vector3 GetOrigin(int index) => Vector3.zero;

        /// <returns>Starting rotation of the projectile at <paramref name="index"/></returns>
        protected virtual Quaternion GetRotation(int index) => Quaternion.identity;

        #endregion Parameters
    }
}