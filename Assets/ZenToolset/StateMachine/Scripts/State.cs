using UnityEngine;

namespace Junnav.Zen.Toolset.StateMachine
{
    /// <summary>
    /// Holds logic for the current state. Inherit from this class to create a new state.
    /// </summary>
    public abstract class State : MonoBehaviour
    {
        /// <summary>
        /// The current state machine responsible for this state
        /// </summary>
        public StateMachine StateMachine { get; set; }

        /// <summary>
        /// Check if the state machine is currently running this state
        /// </summary>
        protected bool IsCurrentState
        {
            get
            {
                if (StateMachine == null) return false;
                return StateMachine.CurrentState == this;
            }
        }

        /// <summary>
        /// Tells the state machine to change into this new state. Pass in 'null' to stop state machine.
        /// </summary>
        /// <param name="nextState">New state to change to. 'null' to stop state machine.</param>
        protected void ChangeState(State nextState)
        {
            if (!IsCurrentState) return;
            StateMachine.ChangeState(nextState);
        }

        /// <summary>
        /// Called when the state is first activated
        /// </summary>
        public virtual void OnStateEnter() { }

        /// <summary>
        /// Called during Update() when this state is currently active
        /// </summary>
        public virtual void OnStateExecute() { }

        /// <summary>
        /// Called during FixedUpdate() when this state is currently active
        /// </summary>
        public virtual void OnStatePhysicsExecute() { }

        /// <summary>
        /// Called during LateUpdate() when this state is currently active
        /// </summary>
        public virtual void OnStatePostExecute() { }

        /// <summary>
        /// Called when the state is being deactivated
        /// </summary>
        public virtual void OnStateExit() { }
    }
}
