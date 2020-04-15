using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





[CreateAssetMenu(fileName = "New Item", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{

    [Serializable]
    public struct ItemAmount
    {
        public Item item;
        [Range(1, 999)]
        public int Amount;
    }

    //public Item item;

    public int energyUsed;
    public List<ItemAmount> Materials;
    public List<ItemAmount> Results;

    public bool CanCraft(ItemContainer itemContainer)
    {
        
        return HasMaterials(itemContainer); // && HasSpace(itemContainer);
    }


    public bool HasMaterials(ItemContainer itemContainer)
    {
     
        foreach (ItemAmount itemAmount in Materials)
        {
           
            
            if (itemContainer.ItemCount(itemAmount.item.name) < itemAmount.Amount)
            {
           
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
        if (Generators.currentEnergy >= energyUsed)
        {
            if (CanCraft(itemContainer))
            {
                Generators.currentEnergy -= energyUsed;
                UpdateInterface.instance.Update();
                RemoveMaterials(itemContainer);
                AddResults(itemContainer);
            }
        }
    }

    public void RemoveMaterials(ItemContainer itemContainer)
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
