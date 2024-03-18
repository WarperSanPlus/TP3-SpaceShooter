using Emetters;
using Extensions;
using Serializables;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// Class that alternates the pattern used by following a list of patterns
    /// </summary>
    public class PatternController : ConstantController
    {
        #region BaseController

        /// <inheritdoc/>
        protected override float OnTimerEnded()
        {
            this.AdvancePattern();
            return base.OnTimerEnded();
        }

        /// <inheritdoc/>
        protected override BaseEmetter[] OnStart()
        {
            var emetters = new BaseEmetter[this.patterns.Length];

            for (var i = 0; i < emetters.Length; i++)
                emetters[i] = this.patterns[i].emetter;

            return emetters;
        }

        /// <inheritdoc/>
        public override void OnReset()
        {
            // Disable if no pattern
            if (this.patterns == null || this.patterns.Length == 0)
            {
                this.enabled = false;
                return;
            }

            this.pattern = this.patterns[0];
            base.OnReset();
        }

        #endregion BaseController

        #region ConstantController

        /// <inheritdoc/>
        protected override void OnStateAdvanced(State state)
        {
            if (this.currentState == State.Start)
                this.startActions.CallActions();
            else if (this.currentState == State.End)
                this.endActions.CallActions();
        }

        #endregion ConstantController

        #region Patterns

        [Header("Patterns")]
        [SerializeField, Tooltip("Determines the patterns this controller will use")]
        private BulletPattern[] patterns = null;

        private int index = -1;

        /// <summary>
        /// Advances the current pattern of the controller
        /// </summary>
        private void AdvancePattern()
        {
            if (this.patterns.Length == 0)
                return;

            // Increase index
            this.index++;

            if (this.index >= this.patterns.Length)
                this.index = 0;

            this.pattern = this.patterns[this.index];
        }

        #endregion Patterns

        #region Actions

        [Header("Actions")]
        [SerializeField, Tooltip("Actions called when any pattern starts")]
        private EmetterAction[] startActions = null;

        [SerializeField, Tooltip("Actions called when any pattern ends")]
        private EmetterAction[] endActions = null;

        #endregion Actions
    }
}