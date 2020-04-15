using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBact : Interactable
{

    public GameObject bottleEmpty;
    public GameObject bottleFull;

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
        bottleEmpty.SetActive(false);
        bottleFull.SetActive(true);

        Interactable.bottleFull = true;

        Destroy(gameObject);    // Destrói o item da cena
    }
}
