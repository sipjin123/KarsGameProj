using System;

using Common.Fsm;
using Common.Time;

namespace Common.Fsm.Action {
	public class TimedWaitAction : FsmActionAdapter {
		
		private float waitTime;
		private readonly CountdownTimer timer;
		private readonly string timeReference;
		private readonly string finishEvent;
		
		/**
		 * Constructor
		 */
		public TimedWaitAction(FsmState owner, string timeReferenceName, string finishEvent) : base(owner) {
			if(string.IsNullOrEmpty(timeReferenceName)) {
				this.timer = new CountdownTimer(1);
			} else {
				this.timer = new CountdownTimer(1, timeReferenceName); // dummy only, we reset on Init()
			}

			this.finishEvent = finishEvent;
		}
		
		/**
		 * Initializes the action. We provide this action so that we can manage instances of this class in an object pool.
		 */
		public void Init(float waitTime) {
			this.waitTime = waitTime;
		}
		
		public override void OnEnter() {
			if(waitTime <= 0) {
				Finish();
			}
			
			timer.Reset(this.waitTime);
		}
		
		public override void OnUpdate() {
			timer.Update();
			
			if(timer.HasElapsed()) {
				Finish();
			}
		}
		
		private void Finish() {
			GetOwner().SendEvent(finishEvent);
		}
		
		/**
		 * Returns the time duration ratio.
		 */
		public float GetRatio() {
			return timer.GetRatio();
		}
		
	}
}

