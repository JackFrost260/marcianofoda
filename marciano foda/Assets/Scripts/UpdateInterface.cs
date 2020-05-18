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
	public GameObject instru;
	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Update2();
	}

	public void Update2()
	{
		currentEnergy.text = "Sua Energia:" + Generators.currentEnergy;
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			instru.SetActive(!instru.activeSelf);
			if (Time.timeScale == 1)
			{
				Time.timeScale = 0;
			}

			else
			{
				if (Time.timeScale == 0)
				{
					Time.timeScale = 1;
				}
			}
		}
	}
}
