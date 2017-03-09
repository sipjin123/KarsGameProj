using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VeloSlider : MonoBehaviour
{
    public Car_Movement obj;
    void Update()
    {
        GetComponent<Slider>().value = -obj.CurrentTurningForceRatio;
    }
}
