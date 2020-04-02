using UnityEngine;
using UnityEngine.AI;


//[RequireComponent(typeof(ColorOnHover))]
public class Interactable : MonoBehaviour
{

	public float radius = 3f;
	public Transform interactionTransform;

	private bool onCollision;

	

	void Update()
	{
		if (onCollision)    
		{
			if(Input.GetKeyDown(KeyCode.E))
			{ 
				Interact();
			}
		}
	}

	
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			onCollision = true;
		}
	}


	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			onCollision = false;
		}
	}

	// Método para ser subscrito
	public virtual void Interact()
	{

	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(interactionTransform.position, radius);
	}

}