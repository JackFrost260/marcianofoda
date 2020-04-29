using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
	public CraftingRecipe craftingRecipe;
	public Inventory inventory;
	public Text recipeText;
	private string recipeString;

	public void OnCraftButtonClick()
	{
		if (craftingRecipe != null && inventory != null)
		{
			craftingRecipe.Craft(inventory);
		}
	}

	public void MouseEnter()
	{ 
		recipeString = craftingRecipe.Feedback();
		recipeText.text = recipeString;
	}

	public void MouseExit()
	{
		recipeString = null;
		recipeText.text = recipeString;
	}

	//private void OnGUI()
	//{
		//GUI.TextArea(new Rect(gameObject.transform.position.x - 180, gameObject.transform.position.y, 120, 60), recipeString);
	//}
}
