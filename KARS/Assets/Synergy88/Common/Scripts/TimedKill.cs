using UnityEngine;
using System.Collections;

namespace Common {
	public class TimedKill : MonoBehaviour {

		private SwarmItem swarm;

		void Awake() {
			this.swarm = GetComponent<SwarmItem>();
			Assertion.AssertNotNull(this.swarm);
		}

		/**
		 * Runs a timed kill using the specified duration.
		 */
		public void Run(float duration) { 
			StartCoroutine(KillAfterDuration(duration));
		}

		private IEnumerator KillAfterDuration(float duration) {
			yield return new WaitForSeconds(duration);

			this.swarm.Kill();
		}

	}
}
