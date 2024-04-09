using Emetters;
using Serializables;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// Class that constantly follows one pattern
    /// </summary>
    public class ConstantController : BaseController
    {
        #region Pattern

        [SerializeField, Tooltip("Pattern used by this controller")]
        protected BulletPattern pattern;

        #endregion Pattern

        #region BaseController

        /// <inheritdoc/>
        protected override BaseEmetter[] OnStart() => new BaseEmetter[] { this.pattern.emetter };

        /// <inheritdoc/>
        protected override float GetStartingTimer() => this.pattern.startTime;

        /// <inheritdoc/>
        protected override void OnTimerAdvanced(float timer, float elapsed)
        {
            if (this.pattern.emetter == null)
                return;

            _ = this.pattern.emetter.Tick(elapsed, this.currentState == State.Attack);
        }

        /// <inheritdoc/>
        protected override float OnTimerEnded()
        {
            // Increase state
            this.AdvanceState();

            // Set timer depending on the current state
            return this.currentState switch
            {
                State.Start => this.pattern.startTime,
                State.Attack => this.pattern.duration,
                _ => this.pattern.exitTime,
            };
        }

        #endregion BaseController

        #region States

        /// <summary>
        /// Current state of the controller
        /// </summary>
        protected State currentState;

        /// <summary>
        /// Possible states of the controller
        /// </summary>
        protected enum State
        { Start, Attack, End }

        /// <summary>
        /// Advances the current state of the controller. The state will follow the order in the enum and loop back at the end
        /// </summary>
        private void AdvanceState()
        {
            this.currentState++;

            if (this.currentState > State.End)
                this.currentState = State.Start;

            if (this.pattern.emetter != null)
            {
                if (this.currentState == State.Start)
                    this.pattern.emetter.OnStart();
                else if (this.currentState == State.End)
                    this.pattern.emetter.OnEnd();
            }

            this.OnStateAdvanced(this.currentState);
        }

        /// <summary>
        /// Called when the state has been advanced
        /// </summary>
        /// <param name="state">New state</param>
        protected virtual void OnStateAdvanced(State state)
        { }

        #endregion States

        #region IResetable

        /// <inheritdoc/>
        public override void OnReset()
        {
            // Disable if emetter is invalid
            if (this.pattern.emetter == null)
            {
                this.enabled = false;
                return;
            }

            this.currentState = State.Start;
            base.OnReset();
        }

        #endregion IResetable
    }
}