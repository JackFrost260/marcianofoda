using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colDetection : MonoBehaviour
{

    public bool trueCollision;

    private void OnCollisionEnter(Collision collision)
    {
        trueCollision = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        trueCollision = false;
    }

}
