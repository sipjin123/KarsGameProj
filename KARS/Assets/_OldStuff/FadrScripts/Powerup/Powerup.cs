using UnityEngine;

namespace Synergy88
{
    public enum PowerupType {

        NULL,


        MISSLE,
        TNT,
        SHEILD,


    }


    public class Powerup : MonoBehaviour
    {
        [SerializeField]
        PowerupType pType;
        [SerializeField]
        private float powerupSpawnTime = 0;
        private float powerupMaxSpawnTime = 10;

        [SerializeField]
        private bool powerUpActive = true;
        private GameRoot _game;


        void Start()
        {
            _game = GameObject.FindObjectOfType<GameRoot>();
            RandomizePowerup();
        }

        void OnTriggerEnter(Collider col)
        {
            return;
            if(col.tag == "Car")
            {
                if (powerUpActive)
                {
                    _game.AcquirePowerup(pType);
                    DisablePowerup();
                }
            }

        }

        void DisablePowerup()
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false ;
            powerUpActive = false;
            powerupSpawnTime = powerupMaxSpawnTime;
        }

        void SpawnPowerup()
        {

            RandomizePowerup(); 
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            powerUpActive = true;
        }

        private void RandomizePowerup()
        {
            pType = (PowerupType) Random.Range(1,5);
        }

        void Update()
        {
            if (!powerUpActive)
            {
                powerupSpawnTime -= Time.deltaTime;
                if(powerupSpawnTime <= 0)
                {
                    SpawnPowerup();
                }
            }
        }
    }
}
