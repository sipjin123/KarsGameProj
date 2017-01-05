using UnityEngine;

using System;
using System.Collections;

using Common;
using Common.Query;
using Common.Signal;

using UniRx;

namespace Synergy88 {

	public class LoginRoot : Scene {

		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();

			this.AddButtonHandler(EButtonType.Login, (ButtonClickedSignal signal) => {
				this.Login();
			});

			this.AddButtonHandler(EButtonType.LoginFacebook, (ButtonClickedSignal signal) => {
                this.Publish(new FacebookLoginRequestSignal());
			});
		}

		protected override void OnEnable() {
			base.OnEnable();

            this.Receive<FacebookLoginSuccessSignal>()
                .Subscribe(_ => OnFacebookLoginSuccessful())
                .AddTo(OnDisableDisposables);

            this.Receive<FacebookLoginFailureSignal>()
                .Subscribe(_ => OnFacebookLoginFailed())
                .AddTo(OnDisableDisposables);
		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}

		private void Login() {
            // load preloader here
            this.Publish(new LoadHomeSignal());
		}

		#region Signals

		private void OnFacebookLoginSuccessful() {
			//string fbId = (string)QuerySystem.Query<string>(QueryIds.UserFacebookId);
			//string fbEmail = (string)QuerySystem.Query<string>(QueryIds.UserEmail);
			//string fbFullname = (string)QuerySystem.Query<string>(QueryIds.UserFullName);
			//string fbProfilePhoto = (string)QuerySystem.Query<string>(QueryIds.UserProfilePhoto);
			this.Login();
		}

		private void OnFacebookLoginFailed() {
			
		}

		#endregion
	}

}