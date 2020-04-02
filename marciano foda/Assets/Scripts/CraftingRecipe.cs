using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





[CreateAssetMenu(fileName = "New Item", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{

    private Inventory inventory;


    [Serializable]
    public struct ItemAmount
    {
        public Item item;
        [Range(1, 999)]
        public int Amount;
    }

    //public Item item;


    public List<ItemAmount> Materials;
    public List<ItemAmount> Results;

    public bool CanCraft(ItemContainer itemContainer)
    {
        Debug.Log("4");
        return HasMaterials(itemContainer); // && HasSpace(itemContainer);
    }


    private bool HasMaterials(ItemContainer itemContainer)
    {
        Debug.Log("5");
        foreach (ItemAmount itemAmount in Materials)
        {
            Debug.Log("6");
            Debug.Log(itemAmount.Amount);
            Debug.Log(itemAmount.item.name);
            Debug.Log(itemContainer);
            
            if (itemContainer.ItemCount(itemAmount.item.name) < itemAmount.Amount)
            {
                Debug.Log("7");
                Debug.LogWarning("You don't have the required materals.");
                return false;
            }
        }
        return true;
    }

   // private bool HasSpace(ItemContainer itemContainer)
    //{
       // foreach (ItemAmount itemAmount in Results)
      //  {
         //   if (!itemContainer.CanAddItem(itemAmount.item, itemAmount.Amount))
          //  {
             //   Debug.LogWarning("Your inventory is full.");
             //   return false;
           // }
       // }
      //  return true;
  //  }

    public void Craft(ItemContainer itemContainer)
    {
        Debug.Log("3");
        if (CanCraft(itemContainer))
        {
            RemoveMaterials(itemContainer);
            AddResults(itemContainer);
        }
    }

    private void RemoveMaterials(ItemContainer itemContainer)
    {
        foreach (ItemAmount itemAmount in Materials)
        {
            for (int i = 0; i < itemAmount.Amount; i++)
            {
               // ItemAmount oldItem = itemContainer.RemoveItem(itemAmount.item.name) ;
                Inventory.instance.Remove(itemAmount.item);
            }
        }
    }
    private void AddResults(ItemContainer itemContainer)
    {
        foreach (ItemAmount itemAmount in Results)
        {
            for (int i = 0; i < itemAmount.Amount; i++)
            {
                Inventory.instance.Add(itemAmount.item);
            }
        }
    }

}
