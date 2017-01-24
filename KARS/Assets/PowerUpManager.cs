using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

    private static PowerUpManager instance;
    public static PowerUpManager Instance
    {
        get { return instance; }
    }

    [SerializeField]
    GameObject BLueCar, RedCar;

    [SerializeField]
    Transform MisslePool;

    public List<GameObject> MissleList;

    void Awake()
    {
        MissleList = new List<GameObject>();
        instance = this;
    }
    void Start()
    {
        BLueCar = GameObject.Find("BlueCar");
        RedCar = GameObject.Find("RedCar");
        StartCoroutine(DelayStartup());
    }

    IEnumerator DelayStartup()
    {
        yield return new WaitForSeconds(1);
        GameObject temp = null;
        
        for (int i = 0; i < MisslePool.childCount; i++)
        {
            temp = MisslePool.GetChild(i).gameObject;
            MissleList.Add(temp);
            temp.GetComponent<MissleScript>().Set_MissleID(i);
        }
    }

    public void LockOnTarget(GameObject _obj)
    {
        try
        {
            MisslePool.transform.GetChild(0).GetComponent<MissleScript>().LockOnToThisObject(_obj);
        }
        catch
        {
            GameObject temp = MissleList[0];
            for (int i = 0; i < MissleList.Count-1; i++)
            {
                MissleList[i] = MissleList[i + 1];
            }
            MissleList[MissleList.Count - 1] = temp;
            MissleList[MissleList.Count - 1].GetComponent<MissleScript>().ResetMissle();
            LockOnTarget(_obj);
        }
    }


	void Update ()
    {
		
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            LockOnTarget(BLueCar);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LockOnTarget(RedCar);
        }
    }
}
