using UnityEngine;
using System.Collections;

public class RayCastShootComplete : MonoBehaviour {

	public int gunDamage = 1;
	public float fireRate = 0.25f;										
	public float weaponRange = 50f;										
	public Transform gunEnd;											

	private Camera fpsCam;												
	private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);	
	private AudioSource gunAudio;										
	private LineRenderer laserLine;										
	private float nextFire;
	private bool fire;


	void Start () 
	{
		fire = false;

		laserLine = GetComponent<LineRenderer>();

		gunAudio = GetComponent<AudioSource>();

		fpsCam = GetComponentInParent<Camera>();

		

	}


	void Update()
	{

		if (Input.GetButtonDown("Fire1")) //&& Time.time > nextFire && Time.timeScale != 0) 
		{
			fire = true;
			gunAudio.Play();
			laserLine.enabled = true;
		}

		if (Input.GetButtonUp("Fire1"))
		{
			fire = false;
			gunAudio.Stop();
			laserLine.enabled = false;
			
		}

		if (fire)
		{
			//nextFire = Time.time + fireRate;

			//StartCoroutine (ShotEffect());
			laserLine.SetPosition(0, gunEnd.position);

			Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));


			RaycastHit hit;

			


			if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
			{
				laserLine.SetPosition(1, hit.point);

				// Get a reference to a health script attached to the collider we hit
				//ShootableBox health = hit.collider.GetComponent<ShootableBox>();

				Cut itemCut = hit.collider.GetComponent<Cut>();

				// If there was a health script attached
				if (itemCut != null)
				{
					// Call the damage function of that script, passing in our gunDamage variable
					itemCut.Cutting();
				}
			}
			else
			{

				laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
			}
		}
	}


	private IEnumerator ShotEffect()
	{
		
		gunAudio.Play ();

		laserLine.enabled = true;

		yield return shotDuration;

		laserLine.enabled = false;
	}

}