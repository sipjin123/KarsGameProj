using UnityEngine;

using System;
using System.Collections;

using Common;
using Common.Query;
using Common.Signal;

using UniRx;

namespace Synergy88 {
	
	public class CurrencyRoot : Scene {

		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();

            this.Receive<UpdatePlayerCurrencySignal>()
                .Subscribe(sig => OnUpdatePlayerCurrency(sig.PlayerCurrency))
                .AddTo(this);

			this.AddButtonHandler(EButtonType.Currency, this.OnLoadCoinScene);
		}

		protected override void OnEnable() {
			base.OnEnable();
		}

		protected override void OnDisable() {
			base.OnDisable();
		}

		private void OnLoadCoinScene(ButtonClickedSignal signal) {
			if (QuerySystem.Query<EScene>(QueryIds.CurrentScene) == EScene.Currency) {
				return;
			}
            
            SystemRoot root = Scene.GetScene<SystemRoot>(EScene.System);

            root.LoadScenePromise<CoinsRoot>(EScene.Coins);
            root.LoadSceneAdditivePromise<CurrencyRoot>(EScene.Currency);
            root.LoadSceneAdditivePromise<BackRoot>(EScene.Back);
        }

        #region Signals

        private void OnUpdatePlayerCurrency(int playerCurrency) {
			int currency = playerCurrency;
		}
		
		#endregion

	}

}