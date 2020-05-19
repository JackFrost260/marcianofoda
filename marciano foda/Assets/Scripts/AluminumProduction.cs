using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AluminumProduction : MonoBehaviour
{
    //public Item Bauxite;
    public GameObject bauxite;
    public GameObject aluminum;
    public Transform positionAluminum;

    public CraftingRecipe craftingRecipe;
    public Inventory inventory;

    public static bool AluminioProduzido = false;


    public void ProduzirAluminum()
    {
        if(craftingRecipe.CraftAluminum(inventory))
        {
            Instantiate(aluminum, positionAluminum.position, Quaternion.identity);
            AluminioProduzido = true;
            bauxite.SetActive(true);
        }
       
    }

    public bool HasMaterial()
    {
        if (craftingRecipe.HasMaterials(inventory) && craftingRecipe.HasEnergia() && !AluminioProduzido)
        {
            return true;
        }

        else return false;
    }

}
