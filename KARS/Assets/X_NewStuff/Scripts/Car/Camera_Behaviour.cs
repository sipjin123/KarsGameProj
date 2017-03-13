using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Behaviour : MonoBehaviour {
    
    public Camera Camera;

    public float Cam_Y;
    public float RotateSpeed;
    void Start()
    {
        RotateSpeed = 25; 
    }
    public void RotateRight(float multiplier)
    {
        lerper = 0;
        Cam_Y += (RotateSpeed * multiplier) * Time.fixedDeltaTime;
        transform.localRotation = Quaternion.Euler( new Vector3(0, Cam_Y, 0));
    }
    public void RotateLeft(float multiplier)
    {
        lerper = 0;
        Cam_Y -= (RotateSpeed * multiplier) * Time.fixedDeltaTime;
        transform.localRotation = Quaternion.Euler( new Vector3(0, Cam_Y, 0));
    }
    float lerper;
    public void ReturnToDefault()
    {
        lerper += .01f;
        Cam_Y = transform.localRotation.y;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, 0), lerper);
    }
}
