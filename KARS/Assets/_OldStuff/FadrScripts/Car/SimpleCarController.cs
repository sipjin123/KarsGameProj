using GameSparks.RT;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Synergy88
{
    public class SimpleCarController : MonoBehaviour {

        #region VARIABLES
        [SerializeField]
        private int playerID = 0;

        [SerializeField]
        private Transform Objective;
        [SerializeField]
        private Transform Car;
        [SerializeField]
        private Camera carCamera;


        [SerializeField]
        private float camDist = 10;

        [SerializeField]
        private float speed = 15;
        private float rotSpeed = 60;

        [SerializeField]
        private float currentRot = 0;

        private float rotTime = 0;

        [SerializeField]
        private Transform carObject;

        [SerializeField]
        private Transform flagCamParent;
        [SerializeField]
        private bool _hasFlag = false;
        public bool hasFlag { get { return _hasFlag; } }

        public float invis = 0;
        private float maxInvis = 1;

        private GameObject[] spawnAreas;
        private bool isAlive = true;
        private float respawnTimer = 0;
        private float maxRespawnTime = 3;

        private GameRoot _game;

        [SerializeField]
        private GameObject shield;
        private bool isShielded = false;
        private float shieldDur = 0;
        private float maxShieldDur = 5;

        #endregion

        #region INITIALIZATION
        void RegisterDataToDebugMode()
        {
            /*
            DebugMode.GetInstance.RegisterDataType(ref maxRespawnTime, "RespawnTime P" + playerID);
            DebugMode.GetInstance.RegisterDataType(ref rotSpeed, "Rotation Speed P" + playerID);
            DebugMode.GetInstance.RegisterDataType(ref speed, "Car Speed P" + playerID);
            DebugMode.GetInstance.RegisterDataType(ref camDist, "CameraDist P" + playerID);
            DebugMode.GetInstance.RegisterDataType(ref maxShieldDur, "ShieldDuration P" + playerID);*/
        }

        public void LoseFlag()
        {
            if (_hasFlag)
            {
                _hasFlag = false;
                carCamera.transform.SetParent(this.transform);
                carCamera.transform.localPosition = new Vector3(0, 2.25f, -3.77f);
                Objective.SetParent(null);
            }
        }

        void Start()
        {
            try
            {
                _game = GameObject.FindObjectOfType<GameRoot>();
                Objective = GameObject.FindGameObjectWithTag("Flag").transform;
                spawnAreas = GameObject.FindGameObjectsWithTag("Respawn");


                RegisterDataToDebugMode();
            }
            catch
            { }
        }
        #endregion
        #region TRIGGER
        void OnTriggerEnter(Collider col)
        {
            if (!_hasFlag && col.tag == "Flag" && invis <= 0)
            {

                _hasFlag = true;
                Objective.transform.SetParent(this.transform);
                carCamera.transform.SetParent(flagCamParent);
                Objective.transform.localPosition = Vector3.zero;
                carCamera.transform.localPosition = new Vector3(0, 2.25f, -3.77f);
                carCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
                invis = maxInvis;

            }
            else if (col.tag == "Wall")
            {
                isAlive = false;
                respawnTimer = maxRespawnTime;
                carObject.gameObject.SetActive(false);
                ResetFlag();
            }
            else if (col.tag == "TNT" || col.tag == "Missle")
            {
                Debug.LogError("BOOM!");

                if (isShielded)
                {
                    Debug.LogError("Surivive");
                }
            }
        }
        #endregion
        void ResetFlag()
        {
            LoseFlag();
            Objective.transform.localPosition = Vector3.zero;
        }

        void Respawn()
        {
            carObject.gameObject.SetActive(true);
            Debug.LogError("respawn position");
            //this.transform.position = spawnAreas[Random.Range(0, spawnAreas.Length)].transform.position;
        }

        //*************************************************************
        //GAMESPARKS
        public Image myHealthBar;
        GameSparks_DataSender _GSDataSender;

        [SerializeField]
        GameObject CollidersToDisable;

        bool NetworkHasStarted;
        #region INITIALIZE NETWORK
        void Awake()
        {
            try
            {
                _GSDataSender = GetComponent<GameSparks_DataSender>();
                _GSDataSender.ObjToTranslate = gameObject;
                _GSDataSender.ObjToRotate = carObject.gameObject;
                _GSDataSender.PlayerCam = carCamera;
            }
            catch { }
        }

        public void StartNetwork()
        {
            if (_GSDataSender.NetworkID == 1)
            {
                transform.position = new Vector3(0, 1.5f, 30);
                myHealthBar = GameSparksManager.Instance.PlayerHealthBar_1;
            }
            else
            {
                transform.position = new Vector3(0, 1.5f, -30);
                myHealthBar = GameSparksManager.Instance.PlayerHealthBar_2;
            }
            if (_GSDataSender.NetworkID.ToString() == GameSparksManager.Instance.PeerID)
            {
                GameSparksManager.Instance.InventoryPanel.SetActive(true);
            }

            _GSDataSender.SendTankMovement(_GSDataSender.NetworkID, transform.position, carObject.transform.eulerAngles);
            NetworkHasStarted = true;
            CurrentHealth = 100;
            inventoryList = new string[3];
        }
        #endregion

        [SerializeField]
        float CurrentHealth;

        private GameSparksRTUnity GetRTSession;
        void SwitchInterpolation(int _bool)
        {
            if(_bool == 0)
            {
                StartCoroutine("DelayExtrapolation");
                return;
            }
            GetRTSession = GameSparksManager.Instance.GetRTSession();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, _GSDataSender.NetworkID);
                data.SetInt(2, _bool);

                GetRTSession.SendData(122, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        IEnumerator DelayExtrapolation()
        {
            
            yield return new WaitForSeconds(.1f);
            _GSDataSender.SendTankMovement(_GSDataSender.NetworkID, transform.position, carObject.transform.eulerAngles);
            yield return new WaitForSeconds(.1f);
            _GSDataSender.SendTankMovement(_GSDataSender.NetworkID, transform.position, carObject.transform.eulerAngles);
            yield return new WaitForSeconds(.1f);
            _GSDataSender.SendTankMovement(_GSDataSender.NetworkID, transform.position, carObject.transform.eulerAngles);


            using (RTData data = RTData.Get())
            {
                data.SetInt(1, _GSDataSender.NetworkID);
                data.SetInt(2, 0);

                GetRTSession.SendData(122, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }

        //--------------------------------------------------------------------------
        //PLAYER INTERACTION
        #region PLAYER INTERACTION
        public void BumpThisObj()
        {
            SwitchInterpolation(2);

            _bumped = true;
            flightForceDelplete = 5;
        }
        public void BumpThisObjWithForce()
        {
            SwitchInterpolation(2);

            _bumped = true;
            flightForceDelplete = 10;
        }
        public void PlayerExplode()
        {
            SwitchInterpolation(2);

            CurrentHealth -= 20;
            myHealthBar.fillAmount = CurrentHealth / 100;
            _GSDataSender.SendHealth(CurrentHealth);

            isFlyng = true;
            flightForceDelplete = 10;
        }
        #endregion
        //--------------------------------------------------------------------------


        public void SetupInteractionVariables(int _isBumped,int _isFlying, int _isFalling,float _force)
        {
            //if (_GSDataSender.NetworkID.ToString() != GameSparksManager.Instance.PeerID)
            {
                if (_isBumped == 1)
                    _bumped = true;
                else
                    _bumped = false;

                if (_isFlying == 1)
                    isFlyng = true;
                else
                    isFlyng = false;

                if (_isFalling == 1)
                    isFalling = true;
                else
                    isFalling = false;
                flightForceDelplete = _force;
            }
        }

        #region PLAYER EXPLOSION
        public bool _bumped;
        public bool isFlyng;
        public bool isFalling;
        float flightForceDelplete;
        Rigidbody _rigidbody;
        Rigidbody _carRigidBody;
        #endregion


        string[] inventoryList;
        void SendInventory()
        {

            GameSparksManager.Instance.SetupInventory(inventoryList[0], inventoryList[1], inventoryList[2]);
            return;
            GameSparksRTUnity GetRTSession;
            GetRTSession = GameSparksManager.Instance.GetRTSession();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, 1);

                data.SetString(2, inventoryList[0]);
                data.SetString(3, inventoryList[1]);
                data.SetString(4, inventoryList[2]);
                GetRTSession.SendData(121, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }

        #region TEST CONTROLS
        void GameTEsting()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (GameSparksManager.Instance.PeerID == "1")
                {
                    BumpThisObjWithForce();

                    GameSparksRTUnity GetRTSession;
                    GetRTSession = GameSparksManager.Instance.GetRTSession();
                    using (RTData data = RTData.Get())
                    {
                        data.SetInt(1, 2);
                        data.SetInt(2, 1);
                        GetRTSession.SendData(117, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                    }
                }
                else if (GameSparksManager.Instance.PeerID == "2")
                {
                    BumpThisObjWithForce();

                    GameSparksRTUnity GetRTSession;
                    GetRTSession = GameSparksManager.Instance.GetRTSession();
                    using (RTData data = RTData.Get())
                    {
                        data.SetInt(1, 1);
                        data.SetInt(2, 1);
                        GetRTSession.SendData(117, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                PlayerExplode();
            }

            if(GameSparksManager.Instance.PeerID == _GSDataSender.NetworkID.ToString())
            {
                if (Input.GetKeyDown(KeyCode.Minus))
                {
                    CurrentHealth -= 20;
                    myHealthBar.fillAmount = CurrentHealth / 100;
                    _GSDataSender.SendHealth(CurrentHealth);
                }
                if (Input.GetKeyDown(KeyCode.Equals))
                {
                    CurrentHealth += 20;
                    myHealthBar.fillAmount = CurrentHealth / 100;
                    _GSDataSender.SendHealth(CurrentHealth);
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    inventoryList[0] = "TNT";
                    SendInventory();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    inventoryList[1] = "Shield";
                    SendInventory();
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    inventoryList[2] = "Smoke";
                    SendInventory();
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                { 
                    inventoryList[0] = "None";
                    inventoryList[1] = "None";
                    inventoryList[2] = "None";
                    SendInventory();
                }
            }
        }
        #endregion
        //*************************************************************
        void FixedUpdate()
        {
            GameTEsting();

    


            try
            {
                if (NetworkHasStarted)
                {
                    if (int.Parse(GameSparksManager.Instance.PeerID) == _GSDataSender.NetworkID)
                    {
                        _GSDataSender.SendTankMovement(_GSDataSender.NetworkID, transform.position, carObject.transform.eulerAngles);
                        _GSDataSender.SendInteractStatus(_GSDataSender.NetworkID, _bumped == true ? 1 : 0, isFlyng == true ? 1 : 0, isFalling == true ? 1 : 0, flightForceDelplete);
                        if (_bumped)
                        {
                            flightForceDelplete--;
                            transform.position += -carObject.transform.forward * 5;
                            transform.position += carObject.transform.up * 2;
                            if (flightForceDelplete <= 0)
                            {
                                _bumped = false;
                                isFalling = true;
                                SwitchInterpolation(0);
                            }
                            return;
                        }
                        if (isFlyng)
                        {
                            flightForceDelplete--;
                            transform.position += carObject.transform.up * 2;
                            if (flightForceDelplete <= 0)
                            {
                                isFlyng = false;
                                isFalling = true;
                                SwitchInterpolation(0);
                            }
                            return;
                        }
                        if (isFalling)
                        {
                            if (transform.position.y > 1)
                            {
                                transform.position += -transform.up;
                            }
                            else
                            {
                                isFalling = false;
                                transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);

                                SwitchInterpolation(0);
                            }
                            return;
                        }




                        if (Input.GetKey(KeyCode.LeftAlt))
                            transform.position += carObject.transform.forward * (Time.fixedDeltaTime * 55);
                        transform.position += carObject.transform.forward * (Time.fixedDeltaTime * 1);
                        if (Input.GetKey(KeyCode.A))
                        {
                            currentRot -= rotSpeed * Time.deltaTime;
                        }
                        else if (Input.GetKey(KeyCode.D))
                        {
                            currentRot += rotSpeed * Time.deltaTime;
                        }
                        carObject.rotation = Quaternion.Euler(0, currentRot, 0);

                    }
                }
            }
            catch
            { }
        }
        void Update()
        {
            //*************************************************************
            //GAMESPARKS
            //if(false)
            try
            {
                if (_GSDataSender.HasControllableObject == false)//GAME SPARK INITIALIZATION
                    return;
                if (int.Parse(GameSparksManager.Instance.PeerID) != _GSDataSender.NetworkID)//GAME SPARK ID
                    return;

            }
            catch { }

            return;
            if (isFlyng || isFalling || _bumped)
                return;
            //*************************************************************




            if (_game.isPlaying)
            {

                if (isShielded)
                {
                    shieldDur -= Time.deltaTime;
                    if(shieldDur <= 0)
                    {
                        isShielded = false;
                        shield.SetActive(false);
                        //*************************************************************
                        //GAMESPARKS
                        _GSDataSender.ActivateShield(false);
                        //*************************************************************
                    }
                }

                invis -= Time.deltaTime;
                if (isAlive)
                {
                    if (Input.touchCount > 0)
                    {
                        foreach (Touch _touches in Input.touches)
                        {
                            if ((_touches.rawPosition.x > Screen.height / 2))
                            {
                                currentRot += rotSpeed * Time.deltaTime;

                            }
                            else
                            {
                                currentRot -= rotSpeed * Time.deltaTime;
                                //this.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);

                            }
                        }
                    }

                    if (Input.GetKey(KeyCode.A))
                    {
                        currentRot -= rotSpeed * Time.deltaTime;
                        //my_rigid.AddForce(transform.forward * speed / 3);

                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        currentRot += rotSpeed * Time.deltaTime;
                        //this.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
                        //my_rigid.AddForce(transform.forward * speed / 3);

                    }
                    else
                    {
                        //my_rigid.AddForce(transform.forward * speed); //Cannot rotate parent since it disrupts the camera orbit
                    }
                    //

                    if (currentRot > 360)
                    {
                        currentRot = 0;
                    }
                    else if (currentRot < 0)
                    {
                        currentRot = 360;
                    }
                    this.transform.Translate(CarMovement());
                    carObject.rotation = Quaternion.Euler(0, currentRot, 0);


                    if (!_hasFlag)
                    {
                        carCamera.transform.LookAt(Objective);
                        Vector3 pivot = Vector3.Normalize(this.transform.localPosition - Objective.transform.localPosition) * 500;
                        Car.transform.localPosition = pivot;
                        Vector3 newCameraPos = Vector3.Normalize(Car.localPosition - Objective.transform.localPosition) * camDist;
                        carCamera.transform.localPosition = new Vector3(newCameraPos.x, 2.25f, newCameraPos.z);
                    }
                    else
                    {

                        flagCamParent.transform.rotation = Quaternion.Euler(0, currentRot, 0);
                    }
                    if (Objective.parent != this.gameObject.transform)
                    {
                        _hasFlag = false;
                    }
                }
                else
                {
                    //dead
                    respawnTimer -= Time.deltaTime;
                    if (respawnTimer < 0)
                    {
                        isAlive = true;
                        Respawn();
                    }

                }
            }
        }

        internal void ActivateShield()
        {
            //*************************************************************
            //GAMESPARKS
            _GSDataSender.ActivateShield(true);
            //*************************************************************
            isShielded = true;
            shieldDur = maxShieldDur;
            shield.SetActive(true);
        }

        internal void DropTNT()
        {
            PowerUpManager.Instance.DropTNT();

        }

        internal void LaunchMissile()
        {
            PowerUpManager.Instance.LaunchMissle();
        }

        private Vector3 CarMovement()
        {
            Vector3 carVelocity = Vector3.zero;
            if (currentRot >= 0 && currentRot <= 90)
            {
                carVelocity = new Vector3(
                1 - ((90 - currentRot) / 90),
                0,
                1 - (currentRot / 90)) * Time.deltaTime * speed;
            }
            else if (currentRot > 90 && currentRot <= 180)
            {
                carVelocity = new Vector3(
                1 - ((currentRot - 90) / 90),
                0,
               ((90 - (currentRot - 90)) / 90) - 1) * Time.deltaTime * speed;
            }
            else if (currentRot > 180 && currentRot <= 270)
            {
                carVelocity = new Vector3(
                ((90 - (currentRot - 180)) / 90) - 1,
                0,
                ((currentRot - 180) / 90) - 1) * Time.deltaTime * speed;
            }
            else if (currentRot > 270 && currentRot <= 360)
            {
                //360
                carVelocity = new Vector3(
                ((currentRot - 270) / 90) - 1,
                0,
                1 - ((90 - (currentRot - 270)) / 90)) * Time.deltaTime * speed;
            }




            return carVelocity;
        }
    }
}