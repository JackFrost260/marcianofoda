using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
    private bool playerBool = true;
    public GameObject player;
    public GameObject car;
    public GameObject car2;

  public void ChangePlayerInteract()
    {
       if(playerBool)
        {
            car.SetActive(true);
            car2.SetActive(false);
            player.SetActive(false);
            playerBool = false;
        }

       else
        {
            player.SetActive(true);
            car2.SetActive(true);
            car.SetActive(false);
            playerBool = true;
        }
    }
}
