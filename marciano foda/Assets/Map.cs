using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject map;
    public GameObject[] otherUIElements;

    private void Update()
    { 

        if (Input.GetKeyDown(KeyCode.M) && map.activeSelf == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UpdateInterface.CursorLock = false;

            Time.timeScale = 0;

            map.SetActive(true);
          
           
            for (int i = 0; i < otherUIElements.Length; i++)
            {
                otherUIElements[i].gameObject.SetActive(false);
            }
        }

        else if (Input.GetKeyDown(KeyCode.M) && map.activeSelf == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            UpdateInterface.CursorLock = true;

            Time.timeScale = 1;

            map.SetActive(false);
            
            for (int i = 0; i < otherUIElements.Length; i++)
            {
                otherUIElements[i].gameObject.SetActive(true);
            }
        }


    }
        
}
