using UnityEngine;

namespace Emetters
{
    /// <summary>
    /// Class that allows to call only one emetter while having multiple emetters shooting
    /// </summary>
    public class CombinedEmetter : BaseEmetter
    {
        [Header("Combined Emetter")]
        [SerializeField, Tooltip("List of emetters that will be called upon calling this emetter")]
        private BaseEmetter[] emetters = null;

        #region BaseEmetter

        /// <inheritdoc/>
        public override void Init(int targetLayer, System.Guid author, float cooldown = -1)
        {
            foreach (BaseEmetter item in this.emetters)
                item.Init(targetLayer, author, this.cooldown);
        }

        /// <returns>This call causedd at least one of the emetters to fire</returns>
        /// <inheritdoc/>
        public override bool Tick(float elapsed, bool shootOnTimerEnd = true)
        {
            var hasFireOnce = false;
            foreach (BaseEmetter item in this.emetters)
                hasFireOnce |= item.Tick(elapsed, shootOnTimerEnd);

            return hasFireOnce;
        }

        /// <inheritdoc/>
        public override void OnStart()
        {
            foreach (BaseEmetter item in this.emetters)
                item.OnStart();

            base.OnStart();
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            foreach (BaseEmetter item in this.emetters)
                item.OnEnd();

            base.OnEnd();
        }

        #endregion BaseEmetter
    }
}