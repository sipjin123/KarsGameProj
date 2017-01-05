using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UniRx;

using Common;
using Common.Query;
using Common.Signal;

namespace Synergy88 {
	
	public class HomeRoot : Scene {

		private Dictionary<EButtonType, Action> buttonMap;

		protected override void Awake() {
			base.Awake();
			this.buttonMap = new Dictionary<EButtonType, Action>();
		}

		protected override void Start() {
			base.Start();
            
            // add button handlers
            this.AddButtonHandler(EButtonType.MoreGames, (ButtonClickedSignal signal) => {
                this.Publish(new LoadMoreGamesSignal());
            });

			this.AddButtonHandler(EButtonType.Currency, (ButtonClickedSignal signal) => {
			});

			this.AddButtonHandler(EButtonType.Help, (ButtonClickedSignal signal) => {
                QuerySystem.Query<PopupCollectionRoot>(QueryIds.PopupCollection).Show(Popup.Popup001);
            });

			this.AddButtonHandler(EButtonType.Leaderboard, (ButtonClickedSignal signal) => {
			});

			this.AddButtonHandler(EButtonType.Shop, (ButtonClickedSignal signal) => {
                this.Publish(new LoadShopSignal());
            });

			this.AddButtonHandler(EButtonType.Play, (ButtonClickedSignal signal) => {
                this.Publish(new LoadGameSignal());
            });

			this.AddButtonHandler(EButtonType.Settings, (ButtonClickedSignal signal) => {
                this.Publish(new LoadSettingsSignal());
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