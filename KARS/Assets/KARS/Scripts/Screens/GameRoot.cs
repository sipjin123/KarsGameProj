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
        [SerializeField]
        private GameObject[] PowerupButton;
        
        [SerializeField]
        private PowerupType[] currentAcquiredPowerups;
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

            this.AddButtonHandler(EButtonType.Powerup1, (ButtonClickedSignal signal) => {
                OnClickPowerUpButton(1);
            });
            this.AddButtonHandler(EButtonType.Powerup2, (ButtonClickedSignal signal) => {
                OnClickPowerUpButton(2);
            });
            this.AddButtonHandler(EButtonType.Powerup3, (ButtonClickedSignal signal) => {
                OnClickPowerUpButton(3);
            });
            this.AddButtonHandler(EButtonType.Powerup4, (ButtonClickedSignal signal) => {
                OnClickPowerUpButton(4);
            });
            currentAcquiredPowerups = new PowerupType[powerupInventorySize];
        }

        //private Powerup powerupInventory;


        internal void AcquirePowerup(PowerupType pType)
        {
            Debug.LogError(pType.ToString());
            for (int x = 0; x < powerupInventorySize; x++)
            {
                if (currentAcquiredPowerups[x] == PowerupType.NULL)
                {

                    switch (pType)
                    {
                        case PowerupType.MISSLE:
                            currentAcquiredPowerups[x] = pType;
                            break;
                        case PowerupType.TNT:
                            currentAcquiredPowerups[x] = pType;
                            break;
                        case PowerupType.SHEILD:
                            currentAcquiredPowerups[x] = pType;
                            break;
                    }

                    PowerupButton[x].SetActive(true);
                    Debug.LogError("Adding " + pType.ToString());
                    break;
                }
            }


        }

        internal void OnClickPowerUpButton(int v)
        {
            switch (v)
            {

                case 1:
                    ActivePowerup(currentAcquiredPowerups[0]);
                    currentAcquiredPowerups[0] = PowerupType.NULL;
                    PowerupButton[0].SetActive(false);
                    break;
                case 2:
                    ActivePowerup(currentAcquiredPowerups[1]);
                    currentAcquiredPowerups[1] = PowerupType.NULL;
                    PowerupButton[1].SetActive(false);
                    break;
                case 3:
                    ActivePowerup(currentAcquiredPowerups[2]);
                    currentAcquiredPowerups[2] = PowerupType.NULL;
                    PowerupButton[2].SetActive(false);
                    break;
                case 4:
                    ActivePowerup(currentAcquiredPowerups[3]);
                    currentAcquiredPowerups[3] = PowerupType.NULL;
                    PowerupButton[3].SetActive(false);
                    break;
            }

        }

        private void ActivePowerup(PowerupType powerupType)
        {
            switch (powerupType)
            {
                case PowerupType.MISSLE:
                    Debug.LogError("Launch Missle");
                    Player[0].LaunchMissile();
                    break;
                case PowerupType.TNT:
                    Debug.LogError("Drop TNT");
                    Player[0].DropTNT();
                    break;
                case PowerupType.SHEILD:
                    Debug.LogError("Activate Shield");
                    Player[0].ActivateShield();
                    break;

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

    }
		
}