using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ferramentas : MonoBehaviour
{
    public GameObject[] ferramentas;
    public Image iconFerramentas;

    public static string ferramenta;

    private int position;


    // Start is called before the first frame update
    void Start()
    {
        position = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            position++;

            if(position > ferramentas.Length)
            {
                ferramentas[0].SetActive(true);
                ferramentas[2].SetActive(false);
                position = 1;
            }

            for(int i = 0; i < position; i++)
            {
                if(i > 1)
                {
                    ferramentas[i - 2].SetActive(false);
                    ferramentas[i - 1].SetActive(false);
                }

                ferramentas[i].SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            position--;

            if (position <= 0)
            {
                ferramentas[0].SetActive(false);
                ferramentas[2].SetActive(true);
                position = 3;
            }

            for (int i = 0; i < position; i++)
            {
                if (position < 3)
                {
                    ferramentas[2].SetActive(false);
                }

                else if (position == 3)
                {
                    ferramentas[0].SetActive(false);
                    ferramentas[1].SetActive(false);
                }
                  
                ferramentas[i].SetActive(true);
            }
        }
    }
}
