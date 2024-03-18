using Emetters;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers
{
    /// <summary>
    /// Class that manages the player's fire request
    /// </summary>
    public class PlayerController : BaseController
    {
        private const string TARGET_LAYER = "BulletPlayer";

        #region BaseController

        /// <inheritdoc/>
        protected override BaseEmetter[] OnStart() => this.emetter == null
            ? base.OnStart()
            : new BaseEmetter[] { this.emetter };

        /// <inheritdoc/>
        protected override int GetTargetLayerIndex() => LayerMask.NameToLayer(TARGET_LAYER);

        /// <inheritdoc/>
        protected override float OnTimerEnded()
        {
            // If the player is in automatic
            // If the player wants to shoot
            if (this.isAutomaticFire && this.requestFire)
                this.TickEmetter(Time.deltaTime, true);

            return base.OnTimerEnded();
        }

        /// <inheritdoc/>
        protected override void OnTimerAdvanced(float timer, float elapsed) => this.TickEmetter(elapsed, false);

        #endregion BaseController

        #region Emetter

        [SerializeField, Tooltip("Emetter used by this controller")]
        private BaseEmetter emetter;

        /// <summary>
        /// Changes the emetter used to <paramref name="emetter"/>
        /// </summary>
        /// <param name="emetter">New emetter to use</param>
        public void SetEmetter(BaseEmetter emetter)
        {
            this.emetter = emetter;

            this.InitEmetter(this.emetter);
        }

        /// <summary>
        /// Calls <see cref="BaseEmetter.Tick(float, bool)"/> of this emetter
        /// </summary>
        /// <param name="elapsed"><inheritdoc cref="BaseEmetter.Tick(float, bool)"/></param>
        /// <param name="shootOnTimerEnd"><inheritdoc cref="BaseEmetter.Tick(float, bool)"/></param>
        private void TickEmetter(float elapsed, bool shootOnTimerEnd)
        {
            // If no emetter is set
            if (this.emetter == null)
                return;

            // If the player can't shoot
            if (!this.enabled || !this.emetter.enabled)
                return;

            this.emetter.Tick(elapsed, shootOnTimerEnd);
        }

        #endregion Emetter

        #region Fire

        [SerializeField, Tooltip("Determines if the player has to hold to shoot")]
        private bool isAutomaticFire = true;

        private bool requestFire = false;

        public void OnFire(InputAction.CallbackContext ctx)
        {
            this.requestFire = ctx.ReadValueAsButton();

            // Based of this thread: https://forum.unity.com/threads/new-input-system-check-if-a-key-was-pressed.952571/
            if (!ctx.action.WasPerformedThisFrame())
                return;

            // If the player is in automatic mode, skip the call
            if (this.isAutomaticFire)
                return;

            this.TickEmetter(Time.deltaTime, true);
        }

        #endregion Fire
    }
}