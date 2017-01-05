using UnityEngine;
using System.Collections;

using Common.Signal;

namespace Synergy88 {
	
	public class SettingsRoot : Scene {

		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();

			this.AddButtonHandler(EButtonType.Credits, (ButtonClickedSignal signal) => {
                // toggle banner
                this.Publish(new ToggleBannerAdsSignal());

                // test unity ads
                this.Publish(new ShowUnityAdsSignal());
			});

			this.AddButtonHandler(EButtonType.Restore, (ButtonClickedSignal signal) => {
                // toggle interstitals
                this.Publish(new ToggleInterstitialAdsSignal());

				// test unity ads
                this.Publish(new ShowUnityAdsSignal() { Region = "001Region" });
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