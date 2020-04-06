using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, ItemContainer
{

	#region Singleton

	public static Inventory instance;

	void Awake()
	{
		instance = this;
	}

	#endregion

	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;

	public int space = 15;  // Quantidade de espaços para itens

	// Lista atual de itens no inventário
	public List<Item> items = new List<Item>();


	// Adicionar um novo item se houver espaço
	public void Add(Item item)
	{
		if (item.showInInventory)
		{
			if (items.Count >= space)
			{
				Debug.Log("Sem Espaço");
				return;
			}

			items.Add(item);

			if (onItemChangedCallback != null)
				onItemChangedCallback.Invoke();
		}
	}

	// Remover um item
	public void Remove(Item item)
	{
		items.Remove(item);

		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}


	//Quantidades de itens com mesmo nome
	public virtual int ItemCount(string itemName)
	{
		int number = 0;

		for (int i = 0; i < items.Count; i++)
		{
			Item item = items[i];
			if (item != null && item.name == itemName)
		    {
				number += 1;
		    }
		}
		return number;
	}

}
