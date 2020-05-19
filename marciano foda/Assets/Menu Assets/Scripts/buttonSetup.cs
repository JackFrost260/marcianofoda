using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonSetup : MonoBehaviour
{
   
    public GameObject weather1;
    public GameObject fadeObject;
    public GameObject instru;
    public GameObject[] buttons;

    public void startGame()
    {
        //Destroy(weather1);
        SceneManager.LoadScene("Level_Design_V1", LoadSceneMode.Single);
    }
    
    public void exitGame()
    {
        Application.Quit();
    }

    public void Instru()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }

        instru.SetActive(true);
    }
    public void ExitInstu()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(true);
        }

        instru.SetActive(false);

    }

}
