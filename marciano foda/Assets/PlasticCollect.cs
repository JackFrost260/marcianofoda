using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasticCollect : MonoBehaviour
{


    public void Collect()
    {
        PlasticProduction.instance.Collect();
    }
}
