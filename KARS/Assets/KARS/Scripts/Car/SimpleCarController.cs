using System.Collections;
using UnityEngine;

namespace Synergy88
{
    public class SimpleCarController : MonoBehaviour {

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

        void RegisterDataToDebugMode()
        {
            DebugMode.GetInstance.RegisterDataType(ref maxRespawnTime, "RespawnTime P" + playerID);
            DebugMode.GetInstance.RegisterDataType(ref rotSpeed, "Rotation Speed P" + playerID);
            DebugMode.GetInstance.RegisterDataType(ref speed, "Car Speed P" + playerID);
            DebugMode.GetInstance.RegisterDataType(ref camDist, "CameraDist P" + playerID);
            DebugMode.GetInstance.RegisterDataType(ref maxShieldDur, "ShieldDuration P" + playerID);
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
            _game = GameObject.FindObjectOfType<GameRoot>();
            Objective = GameObject.FindGameObjectWithTag("Flag").transform;
            spawnAreas = GameObject.FindGameObjectsWithTag("Respawn");


            RegisterDataToDebugMode();
        }

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
        GameSparks_DataSender _GSDataSender;

        [SerializeField]
        GameObject CollidersToDisable;

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
        bool _bumped;
        public void BumpThisObj()
        {
            if (!isFalling && !isFlyng)
                flightForceDelplete = 100;
            isFlyng = true;

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.AddForce(transform.up * 200);
            _rigidbody.AddForce(-transform.forward * 250);


            carObject.gameObject.AddComponent<Rigidbody>();
            _carRigidBody = carObject.GetComponent<Rigidbody>();
            _carRigidBody.useGravity = false;
            _carRigidBody.constraints = RigidbodyConstraints.FreezePosition;



            CollidersToDisable.SetActive(false);

            _bumped = true;
        }
        public void BumpThisObjWithForce()
        {
            if (!isFalling && !isFlyng)
                flightForceDelplete = 100;
            isFlyng = true;

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.AddForce(transform.up * 500);
            _rigidbody.AddForce(-transform.forward * 250);

            carObject.gameObject.AddComponent<Rigidbody>();
            _carRigidBody = carObject.GetComponent<Rigidbody>();
            _carRigidBody.useGravity = false;
            _carRigidBody.constraints = RigidbodyConstraints.FreezePosition;
            _carRigidBody.AddTorque(new Vector3(Random.RandomRange(1, 5), Random.RandomRange(1, 5), Random.RandomRange(1, 5)), ForceMode.Impulse);



            CollidersToDisable.SetActive(false);


            _bumped = true;
        }

        #region PLAYER EXPLOSION
        bool isFlyng;
        bool isFalling;
        float flightForceDelplete;
        Rigidbody _rigidbody;
        Rigidbody _carRigidBody;
        IEnumerator DelayCarReset()
        {
            yield return new WaitForSeconds(1);
            PlayerReset();
        }
        void PlayerReset()
        {
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            Destroy(carObject.GetComponent<Rigidbody>());
            Destroy(carObject.GetComponent<BoxCollider>());
            carObject.transform.localPosition = Vector3.zero;
            carObject.transform.localEulerAngles = Vector3.zero;
            _rigidbody.constraints = RigidbodyConstraints.None;

            CollidersToDisable.SetActive(true);
            _bumped = false;

        }
        public void PlayerExplode()
        {
            if(!isFalling && !isFlyng)
            flightForceDelplete = 100;
            isFlyng = true;

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.AddForce(transform.up * 500);

            carObject.gameObject.AddComponent<Rigidbody>();
            _carRigidBody = carObject.GetComponent<Rigidbody>();
            _carRigidBody.useGravity = false;
            _carRigidBody.constraints = RigidbodyConstraints.FreezePosition;
            _carRigidBody.AddTorque(new Vector3(Random.RandomRange(1, 5), Random.RandomRange(1, 5), Random.RandomRange(1, 5)), ForceMode.Impulse);
        }
        #endregion
        #region TEST CONTROLS
        void GameTEsting()
        {
            /*
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * 0.5f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * 0.5f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * 0.5f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * 0.5f;
            }*/
            
            if (Input.GetKey(KeyCode.X))
            {
                BumpThisObjWithForce();
                //PlayerExplode();
            }

            if (Input.GetKey(KeyCode.Z))
            {
                PlayerReset();
                _bumped = false;
            }
            if (isFlyng)
            {
                flightForceDelplete--;
                if (flightForceDelplete <= 0)
                {
                    isFlyng = false;
                    isFalling = true;
                    _rigidbody.velocity = Vector3.zero;

                    _carRigidBody.useGravity = true;
                    _carRigidBody.gameObject.AddComponent<BoxCollider>();
                }
            }
            if (isFalling)
            {
                return;
                if (transform.position.y < 2)
                {
                    try
                    {
                        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                        _carRigidBody.constraints = RigidbodyConstraints.None;

                        _carRigidBody.angularVelocity = Vector3.zero;
                        _rigidbody.useGravity = false;
                    }
                    catch
                    { }
                    isFalling = false;
                    StartCoroutine("DelayCarReset");
                }
            }
        }
        #endregion
        //*************************************************************
        void FixedUpdate()
        {
            if (int.Parse(GameSparksManager.Instance.PeerID) == _GSDataSender.NetworkID)
            {
                if (Input.GetKey(KeyCode.LeftAlt))
                    transform.position += carObject.transform.forward * (Time.fixedDeltaTime * 55);
                else
                    transform.position += carObject.transform.forward * (Time.fixedDeltaTime * 25);
            }
        }
        void Update()
        {
            //GameTEsting();
            if (_game.isPlaying)
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

                    _GSDataSender.SendTankMovement(_GSDataSender.NetworkID, transform.position, carObject.transform.eulerAngles);
                }
                catch { }
                //GameTEsting();


                if (Input.GetKey(KeyCode.A))
                {
                    currentRot -= rotSpeed * Time.deltaTime;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    currentRot += rotSpeed * Time.deltaTime;
                }
                carObject.rotation = Quaternion.Euler(0, currentRot, 0);
                return;
                if (isFlyng || isFalling || _bumped)
                    return;
                //*************************************************************


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