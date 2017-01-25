using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synergy88
{
    public enum PowerupType {

        SPEED,
        STAR,

    }


    public class Powerup : MonoBehaviour
    {
        PowerupType pType;
        [SerializeField]
        private float powerupSpawnTime = 0;
        private float powerupMaxSpawnTime = 30;

        [SerializeField]
        private bool powerUpActive = true;



        void OnTriggerEnter(Collider col)
        {

            if(col.tag == "Car")
            {
                if (powerUpActive)
                {
                    col.GetComponent<SimpleCarController>().activatePowerup(pType);
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
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            powerUpActive = true;
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
