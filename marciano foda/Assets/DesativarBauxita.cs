using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesativarBauxita : MonoBehaviour
{
    private float timer = 2;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
                timer =2;
            gameObject.SetActive(false);
            
        }
    }

}

