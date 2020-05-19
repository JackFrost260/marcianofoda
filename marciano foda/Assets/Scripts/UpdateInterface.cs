using System;
using System.Collections;
using System.Collections.Generic;
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

	public Text currentEnergy;
	public Text fullEnergy;

	private void Start()
	{
		
		Update2();
	}

	public void Update2()
	{
		currentEnergy.text = ""+ Generators.currentEnergy;
		fullEnergy.text = "" + Generators.maxCapacity;
	}

	public void Update()
	{ 

	}
}
