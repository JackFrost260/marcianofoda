using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftAtual : MonoBehaviour
{
    public int numeroDoCraft;
    
        
    public void mudarOCraft()
    {
        CraftingRecipe.craftAtual = numeroDoCraft;
    }
}
