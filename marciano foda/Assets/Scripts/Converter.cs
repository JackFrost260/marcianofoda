using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Converter : MonoBehaviour
{
    public static bool InCraft = false;
    public GameObject InventoryUI;
    public GameObject CraftUI;
    public GameObject[] otherUIElements;


    public void activate()
    {
        
      Debug.Log("ativado");
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      Time.timeScale = 0;
      InventoryUI.SetActive(true);
      CraftUI.SetActive(true);
        InCraft = true;

        for (int i = 0; i < otherUIElements.Length; i++)
        {
            otherUIElements[i].gameObject.SetActive(false);
        }
    }

   
}
