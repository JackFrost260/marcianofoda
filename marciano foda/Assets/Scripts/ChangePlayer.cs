using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
    private bool playerBool = true;
    public GameObject player;
    public GameObject car;
    public GameObject car2;

    InventoryUI inventoryUI;
    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.instance;
        inventoryUI = InventoryUI.instance;
        inventory.onItemChangedCallback += inventoryUI.UpdateUI;
    }

    public void ChangePlayerInteract()
    {
       if(playerBool)
        {
            Inventory.currentInventory = "Car";
            inventoryUI.UpdateUIInventory();
            car.SetActive(true);
            car2.SetActive(false);
            player.SetActive(false);
            playerBool = false;
        }

       else
        {
        
            Inventory.currentInventory = "Player";
            InventoryUI.instance.UpdateUI();
            player.SetActive(true);
            car2.SetActive(true);
            car.SetActive(false);
            playerBool = true;
        }
    }
}
