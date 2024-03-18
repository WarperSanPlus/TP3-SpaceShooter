using Bullets;
using Interfaces;
using Singletons;
using System;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// Class that manages an internal timer for the other controllers
    /// </summary>
    public abstract class BaseController : MonoBehaviour, IActivatable, IResetable
    {
        private const string TARGET_LAYER = "BulletEnemy";

        #region MonoBehaviour

        /// <inheritdoc/>
        private void Awake() => this.targetLayer = this.GetTargetLayerIndex();

        /// <inheritdoc/>
        private void FixedUpdate() => this.TimerTick(Time.fixedDeltaTime);

        #endregion MonoBehaviour

        #region IActivatable

        /// <inheritdoc/>
        public void SetActive(bool isActive) => this.enabled = isActive;

        #endregion

        #region IResetable

        /// <inheritdoc/>
        public virtual void OnReset()
        {
            this.timer = this.GetStartingTimer();
            this.InitEmetter(this.OnStart());
        }

        #endregion IResetable

        #region Timer

        /// <summary>
        /// Internal timer used by this controller
        /// </summary>
        private float timer;

        /// <summary>
        /// Advances the timer by <paramref name="elapsed"/>
        /// </summary>
        private void TimerTick(float elapsed)
        {
            // Decrease timer
            this.timer -= elapsed;

            this.OnTimerAdvanced(this.timer, elapsed);

            // If the timer is completed
            if (this.timer <= 0f)
                this.timer = this.OnTimerEnded();
        }

        /// <returns>Initial value of the timer</returns>
        /// <remarks>
        /// This will not be the value of the timer when it loops back
        /// </remarks>
        protected virtual float GetStartingTimer() => 0f;

        /// <summary>
        /// Called whenever the timer runs out
        /// </summary>
        /// <returns>
        /// New value of the timer
        /// </returns>
        protected virtual float OnTimerEnded() => 0f;

        /// <summary>
        /// Called whenever the timer advances
        /// </summary>
        /// <param name="elapsed">Time that passed since the last call</param>
        protected virtual void OnTimerAdvanced(float timer, float elapsed)
        { }

        #endregion Timer

        #region Layer

        /// <summary>
        /// Layer to initialize an emetter to
        /// </summary>
        private int targetLayer;

        /// <returns>
        /// Index of the layer where the controller targets
        /// </returns>
        protected virtual int GetTargetLayerIndex() => LayerMask.NameToLayer(TARGET_LAYER);

        #endregion Layer

        #region Author

        /// <summary>
        /// Unique identifiant of this controller
        /// </summary>
        private Guid author = Guid.NewGuid();

        /// <summary>
        /// Destroys all the bullets created by this controller
        /// </summary>
        public void DestroyBullets() => ObjectPool.Instance.DisableObjects(
                BaseBullet.NAMESPACE,
                o => o.TryGetComponent(out BaseBullet bullet) && bullet.Author == this.author
                );

        #endregion Author

        #region Emetter

        /// <summary>
        /// Call <see cref="Emetters.BaseEmetter.Init(int, Guid, float)"/> of every given emetter
        /// </summary>
        /// <param name="emetters">Emetters to initialize</param>
        protected void InitEmetter(params Emetters.BaseEmetter[] emetters)
        {
            foreach (Emetters.BaseEmetter item in emetters)
                item.Init(this.targetLayer, this.author);
        }

        /// <summary>
        /// Called when this controller is initialized
        /// </summary>
        /// <returns>Emetters to initialize</returns>
        protected virtual Emetters.BaseEmetter[] OnStart() => null;

        #endregion Emetter
    }
}