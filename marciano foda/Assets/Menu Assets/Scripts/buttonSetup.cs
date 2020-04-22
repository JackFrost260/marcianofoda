using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonSetup : MonoBehaviour
{
   
    public GameObject weather1;

    public void startGame()
    {
        Destroy(weather1);
        SceneManager.LoadScene("Level_Design_V1", LoadSceneMode.Single);
    }
    
    public void exitGame()
    {
        Application.Quit();
    }

}
