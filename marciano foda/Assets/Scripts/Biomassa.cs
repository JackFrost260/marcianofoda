using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biomassa : MonoBehaviour
{

    public Inventory inventory;
    public Item item;
    public int amount;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateEnergy()
    {
        if (HasMaterials(inventory) && Generators.currentEnergy < Generators.maxCapacity)
        {
            RemoveMaterials(inventory);

            Generators.currentEnergy += 10;

            if(Generators.currentEnergy > Generators.maxCapacity)
            {
                Generators.currentEnergy = Generators.maxCapacity;
            }

            UpdateInterface.instance.Update2();
        }

    }

    public bool HasMaterials(ItemContainer itemContainer)
    {

        if (itemContainer.ItemCount(item.name) < amount)
        {

            Debug.LogWarning("You don't have the required materals.");
            return false;
        }

        return true;
    }

    public void RemoveMaterials(ItemContainer itemContainer)
    {

        for (int i = 0; i < amount; i++)
        {
            Inventory.instance.Remove(item);
        }
    }

}
