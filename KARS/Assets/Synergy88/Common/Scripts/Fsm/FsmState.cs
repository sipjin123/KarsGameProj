using System;
using System.Collections.Generic;

namespace Common.Fsm {
	/**
	 * Interface for an FSM state. State is a collection of actions and transitions.
	 */
	public interface FsmState {
		
		/**
		 * Returns the name of the state.
		 */
		string GetName();
		
		/**
		 * Adds a transition state.
		 */
		void AddTransition(string eventId, FsmState destinationState);
		
		/**
		 * Returns the transition state with the specified event. Returns null if no such transition exists.
		 */
		FsmState GetTransition(string eventId);
		
		/**
		 * Adds an action.
		 */
		void AddAction(FsmAction action);
		
		/**
		 * Returns the collection of actions of this state.
		 */
		IEnumerable<FsmAction> GetActions();
		
		/**
		 * Sends an event to its FSM owner.
		 */
		void SendEvent(string eventId);
		
	}
}

