using UnityEngine;
using System.Collections;

public class RayViewerComplete : MonoBehaviour {

    public float weaponRange = 50f;                       // Distance in Unity units over which the Debug.DrawRay will be drawn
    private Camera fpsCam;// Holds a reference to the first person camera


    void Start () 
    {
        fpsCam = GetComponentInParent<Camera>();
    }

	
	void Update () 
    {
    
        Vector3 lineOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
       
        Debug.DrawRay(lineOrigin, fpsCam.transform.forward * weaponRange, Color.green);
	}
}
