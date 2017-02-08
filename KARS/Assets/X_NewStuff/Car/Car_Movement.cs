using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Movement : MonoBehaviour {

    private float movementSpeed = 5f;

    CharacterController _characterController;

    float currentRotation_Y;
    float rotationSpeed = 55f;

    public Transform CarRotationObject;

    [SerializeField]
    TrailCollision _trailCollision;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        currentRotation_Y = CarRotationObject.transform.eulerAngles.y;
    }

	void Start ()
    {
		
	}
	
	void FixedUpdate ()
    {
        _characterController.Move(CarRotationObject.transform.forward * movementSpeed * Time.fixedDeltaTime);
        _trailCollision._Render();

        InputSystem();


	}
    void InputSystem()
    {
        if(Input.touchCount > 0)
        {
            if(Input.GetTouch(0).position.x > Screen.width*.5f )
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
        if(Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
        if(Input.GetKey(KeyCode.Mouse0))
        {
            if(Input.mousePosition.x > Screen.width *.5f)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
    }

    void MoveRight()
    {
        currentRotation_Y += rotationSpeed * Time.fixedDeltaTime;
        CarRotationObject.transform.eulerAngles = new Vector3(0,currentRotation_Y,0);
    }
    void MoveLeft()
    {
        currentRotation_Y -= rotationSpeed * Time.fixedDeltaTime;
        CarRotationObject.transform.eulerAngles = new Vector3(0, currentRotation_Y, 0);
    }
}
