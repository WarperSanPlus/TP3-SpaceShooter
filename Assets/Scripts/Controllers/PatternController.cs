using Emetters;
using Extensions;
using Serializables;
using System.Collections.Generic;
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
            var emetters = new List<BaseEmetter>();

            for (var i = 0; i < this.patterns.Length; i++)
            {
                BaseEmetter emetter = this.patterns[i].emetter;

                // Skip invalid emetters
                if (emetter == null)
                    continue;

                emetters.Add(emetter);
            }

            return emetters.ToArray();
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
        private BulletPattern[] patterns;

        private int index = -1;

        /// <summary>
        /// Advances the current pattern of the controller
        /// </summary>
        private void AdvancePattern()
        {
            // Skip if no pattern given
            if (this.patterns.Length == 0)
                return;

            // Increase index
            this.index++;

            // Warp index around
            if (this.index >= this.patterns.Length)
                this.index = 0;

            // Assign new pattern
            this.pattern = this.patterns[this.index];
        }

        #endregion Patterns

        #region Actions

        [Header("Actions")]
        [SerializeField, Tooltip("Actions called when any pattern starts")]
        private EmetterAction[] startActions;

        [SerializeField, Tooltip("Actions called when any pattern ends")]
        private EmetterAction[] endActions;

        #endregion Actions
    }
}