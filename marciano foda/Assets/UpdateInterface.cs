using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UpdateInterface : MonoBehaviour
{

    #region Singleton

    public static UpdateInterface instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public TextMeshProUGUI currentEnergy;

    public void Update()
    {
        currentEnergy.text = "Sua Energia:" + Generators.currentEnergy;
    }
}
