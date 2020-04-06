using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{

	new public string name = "New Item";    // Nome do item
	public Sprite icon = null;              // Ícone do item
	public bool showInInventory = true;

	// Chamado quando o item é pressionado no inventário
	public virtual void Use()
	{
		// Usa o item
		// Alguma coisa acontece
	}

	// Chama este método para remover o item do inventário
	public void RemoveFromInventory()
	{
		Inventory.instance.Remove(this);
	}

	public virtual Item GetCopy()
	{
		return this;
	}

}