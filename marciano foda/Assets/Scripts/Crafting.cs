using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crafting : MonoBehaviour
{
	public CraftingRecipe craftingRecipe;

	//public CraftingRecipe CraftingRecipe
	//{
	//	get { return craftingRecipe; }
		//set { SetCraftingRecipe(value); }
	//}

	public Inventory inventory;

	public void OnCraftButtonClick()
	{
		Debug.Log("1");
		if (craftingRecipe != null && inventory != null)
		{
			Debug.Log("2");
			craftingRecipe.Craft(inventory);
		}
	}

}
