using System;

namespace Common.Fsm {
	/**
	 * Interface for FSM actions.
	 */
	public interface FsmAction {
		
		/**
		 * Returns the state owner of the action.
		 */
		FsmState GetOwner();
		
		/**
		 * Routines on enter of action.
		 */
		void OnEnter();
		
		/**
		 * Routines on update of action.
		 */
		void OnUpdate();
		
		/**
		 * Routines on exit of action.
		 */
		void OnExit();
		
	}
}

