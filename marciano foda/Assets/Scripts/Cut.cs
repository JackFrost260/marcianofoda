using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : MonoBehaviour
{
    public Item item;

    public void Cutting()
    {
        Debug.Log("Picking up " + item.name);
        Inventory.instance.Add(item);   // Adiciona ao inventário

        Destroy(gameObject);    // Destrói o item da cena
    }
}
