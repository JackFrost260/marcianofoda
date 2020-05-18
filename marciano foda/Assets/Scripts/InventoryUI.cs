using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class InventoryUI : MonoBehaviour
{
	#region Singleton

	public static InventoryUI instance;

	void Awake()
	{
		instance = this;
	}

	#endregion
	public GameObject inventoryUIPlayer;  // UI do inventário
	public GameObject inventoryUICar;
	public Transform itemsParent;   // Objeto pai de todos os itens

	public GameObject inventoryUI ;

	Inventory inventory;    // Inventário Atual

	private bool cursorLocked = true;

	void Start()
	{
		inventory = Inventory.instance;
		inventoryUI = inventoryUIPlayer;
		inventory.onItemChangedCallback += UpdateUI;

	}

	// Verifique se devemos abrir / fechar o inventário
	void Update()
	{

		if (Input.GetKeyDown(KeyCode.I) && inventoryUI.activeSelf == false && Time.timeScale != 0)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			UpdateInterface.CursorLock = false;

			Time.timeScale = 0;


			inventoryUI.SetActive(true);
			UpdateUI();

		}

		else if (Input.GetKeyDown(KeyCode.I) && inventoryUI.activeSelf == true && !Converter.InCraft)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			UpdateInterface.CursorLock = true;

			Time.timeScale = 1;

			inventoryUI.SetActive(false);
			UpdateUI();

		}


	}

	public void UpdateUIInventory()
	{
		UpdateUI();
	}

	// Atualiza a UI do inventário by:
	//		- Adicionando itens
	//		- limpando slots vazios
	// Isto é chamadado usando um delegate no inventário.
	public void UpdateUI()
	{ 

		InventorySlot[] slots = inventoryUI.GetComponentsInChildren<InventorySlot>();

		for (int i = 0; i < slots.Length; i++)
		{
			if (i < inventory.items.Count)
			{
				slots[i].AddItem(inventory.items[i]);
			}
			else
			{
				slots[i].ClearSlot();
			}
		}
	}

}
