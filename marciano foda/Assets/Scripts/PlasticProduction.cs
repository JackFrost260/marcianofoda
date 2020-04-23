using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasticProduction : MonoBehaviour
{
    #region Singleton

    public static PlasticProduction instance;

    void Awake()
    {
        instance = this;
    }

    #endregion


    private int bact = 0;
    private int plastic = 0;

    public Item item;

    public GameObject [] activate;
    public GameObject deactivate;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Production()
    {
        if(bact > 0)
        {
            plastic += bact;
            
            Debug.Log("bact:" + bact);
            Debug.Log("plastic:" + plastic);
        }
    }

    public void Collect()
    {
        for (int i = plastic; i > 0; i--)
        {
            Inventory.instance.Add(item);
            plastic--;
        }
    }

    public void Deposit()
    {
        if (Interactable.bottleFull)
        {
            Interactable.bottleFull = false;

            bact++;

            for (int i = 0; i < activate.Length ; i++)
            {
                activate[i].SetActive(true);
            }

            deactivate.SetActive(false);

        }
    }
}
