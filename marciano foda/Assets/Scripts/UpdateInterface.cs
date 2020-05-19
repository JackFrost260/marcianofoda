using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UpdateInterface : MonoBehaviour
{
	public static bool CursorLock = true;
	private bool cursorLocked = true;

	#region Singleton

	public static UpdateInterface instance;

	void Awake()
	{
		instance = this;
	}

	#endregion

	public TextMeshProUGUI currentEnergy;

	private void Start()
	{
		
		Update2();
	}

	public void Update2()
	{
		currentEnergy.text = "Sua Energia:" + Generators.currentEnergy;
	}

	public void Update()
	{ 

	}
}
