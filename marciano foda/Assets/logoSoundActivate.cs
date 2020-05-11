using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logoSoundActivate : MonoBehaviour
{

    AudioSource galactonalta;

    bool logoPlayed;
    CanvasGroup thisGroup;

    void Start()
    {
        logoPlayed = false;
        galactonalta = this.GetComponent<AudioSource>();
        thisGroup = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if(logoPlayed == false && thisGroup.alpha > 0)
        {
            galactonalta.Play();
            logoPlayed = true;
        }
    }
}
