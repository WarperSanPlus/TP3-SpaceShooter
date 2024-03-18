using Interfaces;
using UnityEngine;

namespace UtilityScripts
{
    /// <summary>
    /// Disables the <see cref="GameObject"/> after a certain amount of time
    /// </summary>
    public class TTD : MonoBehaviour, IResetable, IActivatable
    {
        #region MonoBehaviour

        /// <inheritdoc/>
        private void Start() => this.iLifeSpans = this.gameObject.GetComponents<ILifespan>();

        /// <inheritdoc/>
        private void FixedUpdate() => this.LifespanTick(Time.fixedDeltaTime);

        #endregion MonoBehaviour

        #region Lifespan

        [SerializeField, Tooltip("Amount of seconds before the gameobject despawns")]
        private float Lifespan = 0f;

        [SerializeField]
        private float lifespanTimer = 0f;

        private ILifespan[] iLifeSpans;

        private void LifespanTick(float elapsed)
        {
            this.lifespanTimer -= elapsed;

            if (this.lifespanTimer > 0f)
                return;

            foreach (ILifespan item in this.iLifeSpans)
                item.OnLifeEnd();

            this.enabled = false;
        }

        #endregion Lifespan

        #region IResetable

        /// <inheritdoc/>
        public void OnReset() => this.lifespanTimer = this.Lifespan;

        #endregion IResetable

        #region IActivatable

        /// <inheritdoc/>
        public void SetActive(bool isActive) => this.enabled = isActive;

        #endregion IActivatable
    }
}