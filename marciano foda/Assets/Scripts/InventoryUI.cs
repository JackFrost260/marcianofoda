using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class InventoryUI : MonoBehaviour
{

	public GameObject inventoryUI;  // UI do inventário
	public Transform itemsParent;   // Objeto pai de todos os itens

	Inventory inventory;    // Inventário Atual

	private bool cursorLocked = true;

	void Start()
	{
		inventory = Inventory.instance;
		inventory.onItemChangedCallback += UpdateUI;
	}

	// Verifique se devemos abrir / fechar o inventário
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			inventoryUI.SetActive(!inventoryUI.activeSelf);

			if (cursorLocked)
			{
				Cursor.lockState = CursorLockMode.None;
				cursorLocked = false;
			}

			else
			{
				Cursor.lockState = CursorLockMode.Locked;
				cursorLocked = true;
			}
			UpdateUI();
		}
	}

	// Atualiza a UI do inventário by:
	//		- Adicionando itens
	//		- limpando slots vazios
	// Isto é chamadado usando um delegate no inventário.
	public void UpdateUI()
	{
		InventorySlot[] slots = GetComponentsInChildren<InventorySlot>();

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
