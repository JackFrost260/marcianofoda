using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ferramentas : MonoBehaviour
{
    public GameObject[] ferramentas;
    public GameObject[] iconFerramentas;

    public static string ferramenta;


    // Start is called before the first frame update
    void Start()
    {
        ferramenta = "nenhuma";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Ativar(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Ativar(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Ativar(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Ativar(3);
        }

    }

    public void Ativar(int a)
    {
        if (a == 0) ferramenta = "mão";
        if (a == 1) ferramenta = "pote";
        if (a == 2) ferramenta = "laser";
        if (a == 3) ferramenta = "nenhuma";

        Debug.Log(ferramenta);

        for (int i = 0; i < ferramentas.Length; i++)
        {
            if (a == 3)
            {
                ferramentas[i].SetActive(false);
                iconFerramentas[i].SetActive(false);
            }

            else
            {
                if (i == a)
                {
                    ferramentas[i].SetActive(true);
                    iconFerramentas[i].SetActive(true);
                }

                else
                {
                    ferramentas[i].SetActive(false);
                    iconFerramentas[i].SetActive(false);
                }
            }
        }
    }
}
