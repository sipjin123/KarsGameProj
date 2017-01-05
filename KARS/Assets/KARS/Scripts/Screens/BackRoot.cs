using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Common;
using Common.Query;
using Common.Signal;

namespace Synergy88 {

	public class BackRoot : Scene
    {

        #pragma warning disable 0169
        private Dictionary<EButtonType, Action> buttonMap;
        #pragma warning restore 0169
        
        protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();

            SystemRoot root = Scene.GetScene<SystemRoot>(EScene.System);

            this.AddButtonHandler(EButtonType.Back, (ButtonClickedSignal signal) => {
                this.Publish(new LoadHomeSignal());
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