using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayCastInteractable : MonoBehaviour
{
	public float weaponRange;
	private Camera fpsCam;
	public Texture2D pressE;
	public Texture2D shot;
	public Texture2D crossHair;

	private Texture2D texture;

	private void Start()
	{
		texture = crossHair;
		fpsCam = GetComponentInParent<Camera>();
	}

	private void Update()
	{
		

		Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));


		RaycastHit hit;


		if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
		{

			PlasticCollect collect = hit.collider.GetComponent<PlasticCollect>();
			PlasticDeposit deposit = hit.collider.GetComponent<PlasticDeposit>();
			Interactable interact = hit.collider.GetComponent<Interactable>();
			Crafting craft = hit.collider.GetComponent<Crafting>();
			Biomassa bio = hit.collider.GetComponent<Biomassa>();
			Converter converter = hit.collider.GetComponent<Converter>();
			ChangePlayer change = hit.collider.GetComponent<ChangePlayer>();
			Cut itemCut = hit.collider.GetComponent<Cut>();


			if (collect != null || deposit != null || interact != null || craft != null || bio != null || converter != null || change != null || itemCut != null)
			{

				if (itemCut != null)
				{
					texture = shot;
				}

				else
				{
			    	texture = pressE;
				}


				if (Input.GetKeyDown(KeyCode.E))
				{
					if (collect != null)
						collect.Collect();

					if (deposit != null)
						deposit.Deposit();

					if (interact != null)
						interact.Interact();

					if (craft != null)
						craft.OnCraftButtonClick();

					if (bio != null)
						bio.GenerateEnergy();

					if (converter != null)
						converter.activate();

					if (change != null)
						change.ChangePlayerInteract();

				}

			}
		}

		else
		{
			texture = crossHair;
		}

	}

	private void OnGUI()
	{
		if (Time.timeScale == 0)
		{
			texture = null;
		}

		GUI.DrawTexture(new Rect((Screen.width / 2) - (100 / 2), (Screen.height / 2) - (100 / 2), 100, 100), texture);
	}
}
