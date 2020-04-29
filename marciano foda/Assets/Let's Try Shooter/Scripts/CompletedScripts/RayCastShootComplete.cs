using UnityEngine;
using System.Collections;

public class RayCastShootComplete : MonoBehaviour {

	public int gunDamage = 1;
	public Transform rayPosition;
	public float fireRate = 0.25f;										
	public float weaponRange = 50f;										
	public Transform gunEnd;											

	private Camera fpsCam;												
	private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);	
	private AudioSource gunAudio;										
	private LineRenderer laserLine;										
	private float nextFire;
	public Texture2D CrossHair;


	void Start () 
	{
		laserLine = GetComponent<LineRenderer>();

		gunAudio = GetComponent<AudioSource>();

		fpsCam = GetComponentInParent<Camera>();

	}
	

	void Update () 
	{
		
		if (Input.GetButtonDown("Fire1") && Time.time > nextFire && Time.timeScale != 0) 
		{
		
			nextFire = Time.time + fireRate;

			
            StartCoroutine (ShotEffect());

            //Vector3 rayOrigin = rayPosition.position;
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));

         
            RaycastHit hit;

			laserLine.SetPosition (0, gunEnd.position);

		
			if (Physics.Raycast (rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
			{
				laserLine.SetPosition (1, hit.point);

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
				
                laserLine.SetPosition (1, rayOrigin + (fpsCam.transform.forward * weaponRange));
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