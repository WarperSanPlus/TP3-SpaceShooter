using Interfaces;
using System;
using UnityEngine;

namespace Bullets
{
    /// <summary>
    /// Class that manages how the bullets behave
    /// </summary>
    public class BaseBullet : MonoBehaviour, ILifespan
    {
        public const string NAMESPACE = "Bullets";

        #region MonoBehaviour

        /// <inheritdoc/>
        private void Awake() => this.colliders = this.gameObject.GetComponentsInChildren<Collider2D>();

        /// <inheritdoc/>
        private void FixedUpdate() => this.TickTravel(Time.fixedDeltaTime);

        #endregion MonoBehaviour

        #region Movement

        [Header("Movement")]
        [SerializeField, Min(0), Tooltip("Determines how fast a projectile goes")]
        private float speed = 1.0f;

        [SerializeField, Tooltip("Determines if the projectile will move backwards on the Z axis")]
        private bool moveToZ = true;

        /// <summary>
        /// Advances the position of the projectile
        /// </summary>
        private void TickTravel(float elapsed)
        {
            // Move forward
            Vector3 nextPos = this.speed * elapsed * Vector3.up;

            if (this.moveToZ)
                nextPos.z += elapsed;

            this.transform.Translate(nextPos, Space.Self);
        }

        #endregion Movement

        #region Layer

        private Collider2D[] colliders;

        /// <summary>
        /// Sets every <see cref="Collider2D"/> of this bullet on <paramref name="layer"/>
        /// </summary>
        /// <param name="layer">Layer to place the <see cref="Collider2D"/> on</param>
        public void SetLayer(int layer)
        {
            foreach (Collider2D item in this.colliders)
                item.gameObject.layer = layer;
        }

        #endregion Layer

        #region Author

        /// <summary>
        /// Who created this bullet
        /// </summary>
        public Guid? Author = null;

        #endregion Author

        #region ILifespan

        /// <inheritdoc/>
        public void OnLifeEnd() => this.gameObject.SetActive(false);

        #endregion ILifespan
    }
}