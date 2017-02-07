using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Behaviour : MonoBehaviour {

    public Transform CamPos1, CamPos2;
    public Transform CamPivot;
    public Camera Camera;
    Transform NewPosToRefer;

    public enum VIEW
    {
        PERPECTIVE,
        TOP,
    }
    public VIEW _currentView;


    bool OnGoingSwitch;

    float switchTransformViewSpeed = 4;
    float switchRotateViewSpeed = 1;

    void Start ()
    {

		
	}
	
	void FixedUpdate () {
		
        if(Input.GetKeyDown(KeyCode.V))
        {
            SwitchView();
        }

        if(OnGoingSwitch)
        {
            switch(_currentView)
            {
                case VIEW.PERPECTIVE:
                    NewPosToRefer = CamPos1;
                    break;
                case VIEW.TOP:
                    NewPosToRefer = CamPos2;
                    break;
            }

            CamPivot.transform.localEulerAngles = Vector3.Slerp(CamPivot.localEulerAngles, NewPosToRefer.localEulerAngles, switchRotateViewSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(CamPivot.transform.localEulerAngles, NewPosToRefer.transform.localEulerAngles) <= 0)
            {
                OnGoingSwitch = false;
            }
        }
	}

    void SwitchView()
    {
        if(_currentView == VIEW.PERPECTIVE)
        {
            _currentView = VIEW.TOP;
            OnGoingSwitch = true;
        }
        else
        {
            _currentView = VIEW.PERPECTIVE;
            OnGoingSwitch = true;
        }
    }
}
