using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_Behaviour : MonoBehaviour {

    public Transform WAypointParent;
    public GameObject _carRotate;

    public Transform[] Patterns;
    public List<Transform> Waypoints;

    bool ifStart;
    int currentChild;

    public TrailCollision _trailcollision;

    float movementSpeed;
    float rotSpeed;
    TronGameManager _tronGameManager;


    bool isDead; 

    void Start ()
    {
        currentChild = 0;
        SetPatternAndWaypoints(Random.RandomRange(0,Patterns.Length));
        movementSpeed = 5;
        rotSpeed = 5;
    }
	
    void SetPatternAndWaypoints(int _pattternVAl)
    {

        movVal.text = _pattternVAl.ToString();
        _tronGameManager = TronGameManager.Instance.GetComponent<TronGameManager>();
        Waypoints.Clear();
        ifStart = false;
        for(int i = 0; i < Patterns[_pattternVAl].childCount;i++)
        {
            Waypoints.Add( Patterns[_pattternVAl].GetChild(i).transform);
        }
        currentChild = 0;
        ifStart = true;
        _trailcollision.SetEmiision(true);
    }

	void FixedUpdate ()
    {
        if (!ifStart || isDead)
            return;
        if (Vector3.Distance(transform.position, Waypoints[currentChild].transform.position) > 3)
        {
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[currentChild].transform.position, movementSpeed * Time.fixedDeltaTime);
       
            var lookPos = Waypoints[currentChild].transform.position - transform.position;
            var rotation = Quaternion.LookRotation(lookPos);
            _carRotate.transform.rotation = Quaternion.Slerp(_carRotate.transform.rotation, rotation, Time.deltaTime * rotSpeed);
        }
        else
        {
            if (currentChild < Waypoints.Count-1)
                currentChild++;
            else
                currentChild = 0;
        }
    }
    void OnTriggerEnter(Collider hit)
    {
        if(TronGameManager.Instance.NetworkStart == false)
        if (hit.gameObject.tag == "Trail" || hit.gameObject.name.Contains("Missle")|| (hit.gameObject.tag == "Car" && hit.gameObject.name != gameObject.name))
        {
            if(!SHieldSwithc)
            DIE();
        }
    }


    public bool SHieldSwithc;
    public GameObject AIShield;
    public void ActivateAIShieldFromButton()
    {
        ActivateAIShield(!SHieldSwithc);
    }
    public void ActivateAIShield(bool _switch)
    {
        SHieldSwithc = !SHieldSwithc;
        AIShield.SetActive(_switch);
    }

    public void DIE()
    {
        transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        isDead = true;
        _trailcollision.SetEmiision(false);
        StartCoroutine("delayREspawn");
    }
    IEnumerator delayREspawn()
    {
        yield return new WaitForSeconds(3);
        SetPatternAndWaypoints(Random.RandomRange(0, Patterns.Length));
        transform.position = new Vector3(3, 1, 0);
        isDead = false;
        _trailcollision.SetEmiision(true);
    }







    //==========================================
    public void EnableDisableTest()
    {
        testPanel.SetActive(!testPanel.activeInHierarchy);
    }
    public Text rotVal, movVal, PatternVal;
    public GameObject testPanel;

    public void AddMoveSpeed(float _var)
    {
        movementSpeed += _var;
        movVal.text = movementSpeed.ToString();
    }
    public void AddRotSpeed(float _var)
    {
        rotSpeed += _var;
        movVal.text = rotSpeed.ToString();
    }
    public void SetPatternRand()
    {
        SetPatternAndWaypoints(Random.RandomRange(0, Patterns.Length));
    }
}
