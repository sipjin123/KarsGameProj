using System;
using System.Collections.Generic;

using UnityEngine;

namespace Common.Fsm {
	public class Fsm {
		
		private readonly string name;
		
		private FsmState currentState;
		private readonly Dictionary<string, FsmState> stateMap;
		
		private Dictionary<string, FsmState> globalTransitionMap;
		
		/**
		 * Constructor
		 */
		public Fsm(string name) {
			this.name = name;
			currentState = null;
			stateMap = new Dictionary<string, FsmState>();
		}
		
		/**
		 * Returns the name of the FSM.
		 */
		public string Name {
			get {
				return name;
			}
		}
		
		/**
		 * Adds a state to the FSM.
		 */
		public FsmState AddState(string name) {
			// state names should be unique
			Assertion.Assert(!stateMap.ContainsKey(name), "The FSM already contains a state with the specified name: " + name);
			
			FsmState newState = new ConcreteFsmState(name, this);
			stateMap[name] = newState;
			return newState;
		}
		
		private delegate void StateActionProcessor(FsmAction action);
		
		private void ProcessStateActions(FsmState state, StateActionProcessor actionProcessor) {
			FsmState currentStateOnInvoke = this.currentState;
			
			IEnumerable<FsmAction> actions = state.GetActions();
			foreach(FsmAction action in actions) {
				actionProcessor(action);
				
				if(this.currentState != currentStateOnInvoke) {
					// this means that the action processing caused a state change
					// we don't continue with the rest of the actions
					break;
				}
			}
		}
		
		/**
		 * Starts the FSM with the specified state name as the starting state.
		 */
		public void Start(string stateName) {
			Assertion.Assert(stateMap.ContainsKey(stateName), "FSM does not contain the specified state: " + stateName);
			ChangeToState(stateMap[stateName]);
		}
		
		private void ChangeToState(FsmState state) {
			if(this.currentState != null) {
				// if there's an active current state, we exit that first
				ExitState(this.currentState);
			}
			
			this.currentState = state;
			EnterState(this.currentState);
		}
		
		private void EnterState(FsmState state) {
			ProcessStateActions(state, delegate(FsmAction action) {
				action.OnEnter();
			});
		}
		
		private void ExitState(FsmState state) {
			FsmState currentStateOnInvoke = this.currentState;
			
			ProcessStateActions(state, delegate(FsmAction action) {
				action.OnExit();
				if(this.currentState != currentStateOnInvoke) {
					// this means that the action's OnExit() causes the FSM to change state
					// note that states should not change state on exit
					throw new Exception("State cannot be changed on exit of the specified state.");
				}
			});
		}
		
		/**
		 * Updates the current state.
		 */
		public void Update() {
			if(this.currentState == null) {
				return;
			}
			
			ProcessStateActions(this.currentState, delegate(FsmAction action) {
				action.OnUpdate();
			});
		}
		
		/**
		 * Returns the current state.
		 */
		public FsmState GetCurrentState() {
			return this.currentState;
		}
		
		/**
		 * Sends an event which may cause state change.
		 */
		public void SendEvent(string eventId) {
			Assertion.Assert(!string.IsNullOrEmpty(eventId), "The specified eventId can't be empty.");
			
			if(currentState == null) {
				Debug.LogWarning(string.Format("Fsm {0} does not have a current state. Check if it was started.", this.name));
				return;
			}
			
			FsmState transitionState = ResolveTransition(eventId);
			if(transitionState == null) {
				Debug.LogWarning(string.Format("The current state {0} has no transtion for event {1}.", this.currentState.GetName(), eventId));
			} else {
				ChangeToState(transitionState);
			}
		}
		
		private FsmState ResolveTransition(string eventId) {
			FsmState transitionState = this.currentState.GetTransition(eventId);
			
			if(transitionState == null) {
				// try to get from global transitions
				// note the state transitions have precedence over global transitions
				if(this.globalTransitionMap != null && this.globalTransitionMap.ContainsKey(eventId)) {
					return this.globalTransitionMap[eventId];
				}
			} else {
				// the current state has a transition for the specified event
				return transitionState;
			}
			
			return null;
		}
		
		/**
		 * Adds a global transition
		 */
		public void AddGlobalTransition(string eventId, FsmState destinationState) {
			if(this.globalTransitionMap == null) {
				// we lazy initialize because not all FSMs have global transitions
				this.globalTransitionMap = new Dictionary<string, FsmState>();
			}
			
			Assertion.Assert(!this.globalTransitionMap.ContainsKey(eventId)); // should not contain transition for the specified event yet
			this.globalTransitionMap[eventId] = destinationState;
		}
		
	}
}

