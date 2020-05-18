using System.Collections;
using System.Collections.Generic;
//using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
	public CraftingRecipe[] craftingRecipe;
	public Inventory inventory;

	//public Text recipeText;

	public Text energia;
	public Text namizinho;
	public Image[] recipeIcons;

	public GameObject panelInfo;
	public GameObject inventoryUI;
	public GameObject background;
	public GameObject[] otherUIElements;

	public Button criarButton;


	//private string recipeString;



	public void OnCraftButtonClick()
	{
		if (craftingRecipe != null && inventory != null)
		{
			craftingRecipe[CraftingRecipe.craftAtual-1].Craft(inventory);

			if (!craftingRecipe[CraftingRecipe.craftAtual - 1].HasMaterials(inventory) || !craftingRecipe[CraftingRecipe.craftAtual - 1].HasEnergia())
			{
				criarButton.interactable = false;
			}
		}
	}

	//public void MouseEnter()
	//{ 
		//recipeString = craftingRecipe.Feedback();
		//recipeText.text = recipeString;
	//}

	//public void MouseExit()
	//{
		//recipeString = null;
		//recipeText.text = recipeString;
	//}

	public void MostrarInformações()
	{

		energia.text = craftingRecipe[CraftingRecipe.craftAtual-1].RecipeEnergia().ToUpper(); ;
		namizinho.text = craftingRecipe[CraftingRecipe.craftAtual-1].NameItem().ToUpper(); ;

		recipeIcons[0].sprite = craftingRecipe[CraftingRecipe.craftAtual-1].RecipeIcon1();
		recipeIcons[1].sprite = craftingRecipe[CraftingRecipe.craftAtual-1].RecipeIcon2();

		if(craftingRecipe[CraftingRecipe.craftAtual - 1].HasMaterials(inventory) && craftingRecipe[CraftingRecipe.craftAtual - 1].HasEnergia())
		{
			criarButton.interactable= true;
		}

		else criarButton.interactable = false;
		panelInfo.SetActive(true);

	}

    public void Sair()
	{
		
		Time.timeScale = 1;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = true;
		Converter.InCraft = false;

		for (int i = 0; i < otherUIElements.Length; i++)
		{
			otherUIElements[i].gameObject.SetActive(true);
		}

		
		inventoryUI.SetActive(false);
		panelInfo.SetActive(false);
		background.SetActive(false);
	}



	//private void OnGUI()
	//{
	//GUI.TextArea(new Rect(gameObject.transform.position.x - 180, gameObject.transform.position.y, 120, 60), recipeString);
	//}
}
