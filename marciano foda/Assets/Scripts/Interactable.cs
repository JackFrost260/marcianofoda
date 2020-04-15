using UnityEngine;
using UnityEngine.AI;


//[RequireComponent(typeof(ColorOnHover))]
public class Interactable : MonoBehaviour
{

	public static bool bottleFull = false;
	//public float radius = 3f;
	//public Transform interactionTransform;


	

	void Update()
	{
	
	}


	// Método para ser subscrito
	public virtual void Interact()
	{

	}

	//void OnDrawGizmosSelected()
	//{
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawWireSphere(interactionTransform.position, radius);
	//}

}