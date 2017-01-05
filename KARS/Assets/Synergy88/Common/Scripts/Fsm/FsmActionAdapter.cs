using System;

namespace Common.Fsm {
	/**
	 * An adapter for classes implementing the FsmAction interface.
	 */
	public abstract class FsmActionAdapter : FsmAction {
		
		private readonly FsmState owner;
		
		/**
		 * Constructor
		 */
		public FsmActionAdapter(FsmState owner) {
			this.owner = owner;
		}

		#region FsmAction implementation
		public FsmState GetOwner() {
			return owner;
		}

		public virtual void OnEnter() {
			// may or may not be implemented by deriving class
		}

		public virtual void OnUpdate() {
			// may or may not be implemented by deriving class
		}

		public virtual void OnExit() {
			// may or may not be implemented by deriving class
		}
		#endregion
	}
}

