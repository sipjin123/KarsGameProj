using System;
using System.Collections.Generic;

namespace Common.Fsm {
	/**
	 * A concrete implementation of an FSM state.
	 */
	class ConcreteFsmState : FsmState {
		
		private readonly string name;
		private readonly Fsm owner;
		
		private readonly Dictionary<string, FsmState> transitionMap;
		private readonly List<FsmAction> actionList;
		
		/**
		 * Constructor
		 */
		public ConcreteFsmState(string name, Fsm owner) {
			this.name = name;
			this.owner = owner;
			
			this.transitionMap = new Dictionary<string, FsmState>();
			this.actionList = new List<FsmAction>();
		}

		#region FsmState implementation
		public string GetName() {
			return name;
		}

		public void AddTransition(string eventId, FsmState destinationState) {
			// can't have two transitions for the same event
			Assertion.Assert(!transitionMap.ContainsKey(eventId), string.Format("The state {0} already contains a transition for event {1}.", this.name, eventId));
			transitionMap[eventId] = destinationState;
		}

		public FsmState GetTransition(string eventId) {
			if(transitionMap.ContainsKey(eventId)) {
				return transitionMap[eventId];
			}
			
			return null;
		}

		public void AddAction(FsmAction action) {
			Assertion.Assert(!actionList.Contains(action), "The state already contains the specified action.");
			Assertion.Assert(action.GetOwner() == this, "The owner of the action should be this state.");
			actionList.Add(action);
		}

		public IEnumerable<FsmAction> GetActions() {
			return actionList;
		}

		public void SendEvent(string eventId) {
			this.owner.SendEvent(eventId);
		}
		#endregion
	}
}

