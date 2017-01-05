using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using UniRx;

namespace Synergy88
{
    /// <summary>
    /// This handles playing of common sound effects like button hovers and clicks.
    /// </summary>
    public class AudioRoot : Scene
    {
        /// <summary>
        /// The audio player to be used in playing clips.
        /// </summary>
        [SerializeField]
        private AudioPlayer AudioPlayer;

        protected override void Awake()
        {
            base.Awake();

			Assertion.AssertNotNull(AudioPlayer);
        }

        protected override void Start()
        {
            base.Start();

            // play hover sound effect when a button is hovered
            this.Receive<ButtonHoveredSignal>()
                .Subscribe(_ => AudioPlayer.PlaySFX(SFX.Sfx001))
                .AddTo(this);

            // play click sound effect when a button is clicked
            this.Receive<ButtonClickedSignal>()
                .Subscribe(_ => AudioPlayer.PlaySFX(SFX.Sfx002))
                .AddTo(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }

}
