using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour
{

	public Image icon;
	public Button removeButton;

	Item item;  //Item atual no slot

	// Adicionar item no slot
	public void AddItem(Item newItem)
	{
		item = newItem;

		icon.sprite = item.icon;
		icon.enabled = true;
		removeButton.interactable = true;
	}

	// Limpar o slot
	public void ClearSlot()
	{
		item = null;

		icon.sprite = null;
		icon.enabled = false;
		removeButton.interactable = false;
	}

	// Se o botão remover for pressionado, esta função será chamada.
	public void RemoveItemFromInventory()
	{
		Inventory.instance.Remove(item);
	}

	// Usar o item
	public void UseItem()
	{
		if (item != null)
		{
			item.Use();
		}
	}

}
