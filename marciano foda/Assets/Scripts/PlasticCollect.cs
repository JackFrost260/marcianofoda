using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasticCollect : MonoBehaviour
{
    public GameObject gaveta;

    public void Collect()
    {
        PlasticProduction.instance.Collect();
        gaveta.SetActive(false);
    }
}
