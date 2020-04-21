using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Converter : MonoBehaviour
{
    public GameObject InventoryUI;
    public GameObject CraftUI;


    public void activate()
    {
    
        
            Debug.Log("ativado");
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            InventoryUI.SetActive(true);
            CraftUI.SetActive(true);      
    }

    public void desactivate()
    {
        InventoryUI.SetActive(false);
        CraftUI.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
