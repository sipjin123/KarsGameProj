using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum SplashState {
	HIDDEN,
	FADE_IN,
	STAY,
	FADE_OUT
};

namespace Synergy88 {

	public class SplashRoot : Scene {
		
		[SerializeField]
		private List<Image> images;

		[SerializeField]
		private float fadeIn = 1.5f;
		[SerializeField]
		private float stay = 0.5f;
		[SerializeField]
		private float fadeOut = 1.5f;

		private float currentAlpha;
		private float timer;

		private int currentSplash = 0;

		private Color tempColor;
		private SplashState currentState;

		private bool end = false;

		protected override void Awake() {
			base.Awake();
			Assertion.Assert(this.images.Count > 0);
		}

		protected override void Start() {
			// assert
			Assertion.Assert(!this.images.Find(s => s == null));

            Debug.LogFormat("SplashRoot::Awake HasSceneData:{0} PrevScene:{1}\n", HasSceneData(), GetSceneData<SceneData>().PrevSceneName);

            // prepare the active images
            List<Image> activeImages = this.images.FindAll(s => s.gameObject.activeSelf);
			foreach (Image image in activeImages) {
				image.gameObject.SetActive(false);

				this.tempColor = image.color;
				this.tempColor.a = 0f;

				image.color = this.tempColor;
			}

			this.StartFade(0);
		}

		private void Update() {

			//if (Input.GetKeyDown(KeyCode.Escape)) {
			//	Application.Quit();
			//}

			if (this.end) {
				return;
			}

			this.timer += Time.deltaTime;

			switch (this.currentState) {
			case SplashState.HIDDEN:
				if (this.currentSplash >= this.images.Count - 1) {
					//loadingScreen.loadNextScene();
					//this.LoadSceneAdditive(EScene.Home);
					this.end = true;
                    this.Publish(new SplashDoneSignal());
				} 
				else {
					this.StartFade(this.currentSplash + 1);
				}

				break;

			case SplashState.FADE_IN:
				if (this.timer >= fadeIn) {
					this.SetAlpha(1f);
					this.SetState(SplashState.STAY);
				} 
				else {
					this.SetAlpha(this.timer/fadeIn);
				}

				break;

			case SplashState.STAY:
				if (this.timer >= stay) {
					this.SetState(SplashState.FADE_OUT);
				}

				break;

			case SplashState.FADE_OUT:
				if (this.timer >= fadeOut) {
					this.SetAlpha(0f);
					this.SetState(SplashState.HIDDEN);
				} 
				else {
					this.SetAlpha(1f - (this.timer/fadeOut));
				}
				break;
			}

			if (Input.GetMouseButtonDown(0)) {
				this.SetAlpha(0f);
				this.SetState(SplashState.HIDDEN);
			}
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

		private void StartFade(int index) {
			this.images[index].gameObject.SetActive(true);
			this.tempColor = this.images[index].color;
			this.currentSplash = index;
			this.SetState(SplashState.FADE_IN);
		}

		private void SetAlpha(float a) {
			this.tempColor.a = a;
			this.images[this.currentSplash].color = this.tempColor;
		}

		private void SetState(SplashState state) {
			this.currentState = state;
			this.timer = 0f;
		}
	}

}