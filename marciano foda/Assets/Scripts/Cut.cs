using com.ootii.Graphics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : MonoBehaviour
{
    public Item item;
    public Text feedbackItem;

    public void Cutting()
    {
        TextTime.feedbackString = "+ " + item.name;
        TextTime.textAtivado = true;

        Inventory.instance.Add(item);   // Adiciona ao inventário

        Destroy(gameObject);    // Destrói o item da cena
    }
}
