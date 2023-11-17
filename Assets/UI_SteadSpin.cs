using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SteadSpin : MonoBehaviour
{
    public float SPINSPEED = 1f;
    void FixedUpdate()
    {
        transform.Rotate(0f, 0f, SPINSPEED);        
    }
}
