using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBact : Interactable
{

    public GameObject[] Full;

    public override void Interact()
    {
        base.Interact();

        if (!Interactable.bottleFull)
        {
            PickUp();
        }
    }


    // Update is called once per frame
    void PickUp()
    {
        for (int i = 0; i < Full.Length; i ++)
        {
            Full[i].SetActive(true);
        }

        Interactable.bottleFull = true;

        Destroy(gameObject);    // Destrói o item da cena
    }
}
