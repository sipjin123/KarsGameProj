using UnityEngine;
using System.Collections;

using UniRx;
using System;

namespace Synergy88 {
	
	public class BackgroundRoot : Scene {

        [SerializeField]
        private GameObject backgroundImage;

		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();


            this.Receive<LoadGameSignal>()
                .Subscribe(_ => OnStartGame())
                .AddTo(this);

            this.Receive<GameEndSignal>()
                .Subscribe(_ => OnEndGame())
                .AddTo(this);
        }

        private void OnStartGame()
        {
            backgroundImage.SetActive(false);
        }

        private void OnEndGame()
        {
            backgroundImage.SetActive(true);
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