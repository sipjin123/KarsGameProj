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
        private GameObject[] PowerupButton;
        
        private int powerupInventorySize = 4;

        private bool _isPlaying = true;
        public bool isPlaying { get { return _isPlaying; } }

        private bool gameEnd = false;

        void RegisterDataToDebugMode()
        {
            //DebugMode.GetInstance.RegisterDataType(ref timePerGame, "Time Per Game");

        }

        void Update()
        {
            if (_isPlaying)
            {
                timeLeft -= Time.deltaTime;
                TimeText.text = "Time: " + CalcTimeLeft();
                
                if(timeLeft <= 0 && !gameEnd)
                {
                    _isPlaying = false;
                    gameEnd = true;
                    this.Publish(new GameEndSignal() { Player1Score = Mathf.RoundToInt(p1Score), Player2Score = Mathf.RoundToInt(p2Score) });
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