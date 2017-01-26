using UnityEngine;
using System.Collections;

using Common.Signal;

namespace Synergy88 {
	
	public class ResultsRoot : Scene {

		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();

            SystemRoot root = Scene.GetScene<SystemRoot>(EScene.System);

            this.AddButtonHandler(EButtonType.Score, (ButtonClickedSignal signal) => {
				// score button clicked
			});

			this.AddButtonHandler(EButtonType.Home, (ButtonClickedSignal signal) => {
                // home button clicked
                // load preloader here
                root.LoadScenePromise<HomeRoot>(EScene.Home);
                //root.LoadSceneAdditivePromise<CurrencyRoot>(EScene.Currency);
            });

			this.AddButtonHandler(EButtonType.Ads, (ButtonClickedSignal signal) => {
				// ads button clicked
			});

			this.AddButtonHandler(EButtonType.Leaderboard, (ButtonClickedSignal signal) => {
				// leaderboards button clicked
			});

			this.AddButtonHandler(EButtonType.Shop, (ButtonClickedSignal signal) => {
                // shop button clicked
                root.LoadScenePromise<ShopRoot>(EScene.Shop);
                root.LoadSceneAdditivePromise<CurrencyRoot>(EScene.Currency);
                root.LoadSceneAdditivePromise<BackRoot>(EScene.Back);
            });

			this.AddButtonHandler(EButtonType.Refresh, (ButtonClickedSignal signal) => {
                // refresh button clicked
                this.Publish(new LoadGameSignal());
            });

			this.AddButtonHandler(EButtonType.Upload, (ButtonClickedSignal signal) => {
				// update/share button clicked
			});
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