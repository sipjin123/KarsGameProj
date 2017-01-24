using System;
using UnityEngine;
using UnityEngine.UI;

using UniRx;

namespace Synergy88 {
	
	public class GameRoot : Scene {


        [SerializeField]
        private Text ScoreText;
        [SerializeField]
        private Text ScoreText_p2;
        [SerializeField]
        private Text TimeText;
        

        private float p1Score = 0;
        private float p2Score = 0;
        [SerializeField]
        private float timeLeft = 90;
        private float timePerGame = 90;

        [SerializeField]
        private GameObject[] SpawnPoints;

        [SerializeField]
        private SimpleCarController[] Player;


        private bool _isPlaying = true;
        public bool isPlaying { get { return _isPlaying; } }

        void RegisterDataToDebugMode()
        {
            DebugMode.GetInstance.RegisterDataType(ref timePerGame, "Time Per Game");

        }

        void Update()
        {
            if (_isPlaying)
            {
                timeLeft -= Time.deltaTime;
                TimeText.text = "Time: " + CalcTimeLeft();

                if (Player[0].hasFlag)
                {
                    p1Score += Time.deltaTime;
                    ScoreText.text = "Player 1: " + Mathf.RoundToInt(p1Score);
                }
                else if (Player[1].hasFlag)
                {
                    p2Score += Time.deltaTime;
                    ScoreText_p2.text = "Player 2: " + Mathf.RoundToInt(p2Score);
                }

                if(timeLeft <= 0)
                {
                    _isPlaying = false;
                }

            }
        }

        internal void UnPauseGame()
        {
            _isPlaying = true;
        }

        internal void PauseGame()
        {
            _isPlaying = false;
        }

        private string CalcTimeLeft()
        {
            string timeString = "0:00";

            if (timeLeft > 0)
                timeString = Mathf.FloorToInt(timeLeft / 60) + ":" + Mathf.FloorToInt(timeLeft % 60);

            return timeString;
        }

        protected override void Awake() {

            base.Awake();
            
            ScoreText.text = "Player 1: " + p1Score;
            ScoreText_p2.text = "Player 2: " + p2Score;

        }

        protected override void Start() {

            //Remove Background Signal
            RegisterDataToDebugMode();

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