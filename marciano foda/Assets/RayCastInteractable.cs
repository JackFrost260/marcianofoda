using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayCastInteractable : MonoBehaviour
{
	public float weaponRange;
	public Transform rayPosition;
	private Camera fpsCam;
	public Sprite pressESprite;
	public Image pressImage;

	private void Start()
	{
		fpsCam = GetComponentInParent<Camera>();
	}

	private void Update()
	{

		//Vector3 rayOrigin = rayPosition.position;
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


			if (collect != null || deposit != null || interact != null || craft != null || bio != null || converter != null || change != null)
			{
				//pressImage.enabled = true;
				//pressImage.gameObject.transform.position = Input.mousePosition;
				//pressImage.gameObject.transform.position = new Vector3((Screen.width / 2) - (100 / 2), (Screen.height / 2) - (100 / 2), 0);
				//pressImage.sprite = pressESprite;
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

			//else pressImage.enabled = false;
		}
	}
}
