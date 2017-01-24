using UnityEngine;

using System.Collections;

namespace Synergy88 {
	
	public class GameRoot : Scene {


        private bool p1HasFlag = false;
        private bool p2HasFlag = false;



        private float p1Score = 0;
        private float p2Score = 0;
        private int timeLeft = 90;

        protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();
		}

		protected override void OnEnable() {
			base.OnEnable();
		}

		protected override void OnDisable() {
			base.OnDisable();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}
	}
		
}