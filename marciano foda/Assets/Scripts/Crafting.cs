using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
	public CraftingRecipe craftingRecipe;
	public Inventory inventory;

	public void OnCraftButtonClick()
	{
		if (craftingRecipe != null && inventory != null)
		{
			craftingRecipe.Craft(inventory);
		}
	}

}
