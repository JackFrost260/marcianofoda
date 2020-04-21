using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionUI : MonoBehaviour
{
    public GameObject constructionUI;
	public GameObject BioUI;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			constructionUI.SetActive(!constructionUI.activeSelf);
			BioUI.SetActive(false);
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

			if (UpdateInterface.CursorLock)
			{
				Cursor.lockState = CursorLockMode.None;
				UpdateInterface.CursorLock = false;
			}

			else
			{
				Cursor.lockState = CursorLockMode.Locked;
				UpdateInterface.CursorLock = true;
			}
		}

	}

	public void ActivateBio()
	{
		BioUI.SetActive(true);
	}

}
