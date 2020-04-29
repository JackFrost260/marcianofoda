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
	public static string currentInventory = "Player";
	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;

	public int spacePlayer = 0;  // Quantidade de espaços para itens
	public int spaceCar = 0;
	private int space;

	// Lista atual de itens no inventário
	public List<Item> itemsPlayer = new List<Item>();
	public List<Item> itemsCar = new List<Item>();
	public List<Item> items = new List<Item>();

	private void Start()
	{
		space = spacePlayer;
		items = itemsPlayer;
	}
	// Adicionar um novo item se houver espaço
	public void Add(Item item)
	{
		changeInventory();

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
		changeInventory();

		items.Remove(item);

		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}


	//Quantidades de itens com mesmo nome
	public virtual int ItemCount(string itemName)
	{
		changeInventory();

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

	public void changeInventory()
	{
		if (currentInventory == "Player")
		{
			space = spacePlayer;
			items = itemsPlayer;
		}

		if (currentInventory == "Car")
		{
			space = spaceCar;
			items = itemsCar;
		}
	}

}
