using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingGenerator : MonoBehaviour
{
	public CraftingRecipe craftingRecipe;
	public GameObject generatorPreview;
	public GameObject constructionUI;
	public GameObject generatorImageUI;


	public BuildSystem buildSystem;
	public Inventory inventory;
	public Text GeneratorQuantText;
	public Button constructButton;
	private int GeneratorQuant;


	public void CraftGenerator()
	{
		if (craftingRecipe != null && inventory != null)
		{
			GeneratorQuant += craftingRecipe.CraftGenerator(inventory);
			GeneratorQuantText.text = "" + GeneratorQuant;

			if (GeneratorQuant > 0)
			{
				constructButton.interactable = true;
			}
		}

	}

	public void ConstructionGenerator()
	{
		if (!buildSystem.isBuilding)
		{
			buildSystem.NewBuild(generatorPreview);
			GeneratorQuant -= 1;
			GeneratorQuantText.text = "" + GeneratorQuant;
			constructionUI.SetActive(false);
			Cursor.lockState = CursorLockMode.None;
			Time.timeScale = 1;
			UpdateInterface.CursorLock = true;

			if (GeneratorQuant == 0)
			{
				constructButton.interactable = false;
			}

			generatorImageUI.SetActive(false);

		}
	}
}
