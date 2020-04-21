using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview : MonoBehaviour
{

    public GameObject prefab;

    private MeshRenderer myRend;
    public Material goodMat;
    public Material badMat;

    private BuildSystem buildSystem;

    private bool isSnapped = false;

    private bool inTheGround; 
    private bool colWithObjects;

    //public List<string> tagsISnapTo = new List<string>();//list of all of the SnapPoint tags this particular preview can snap too
    //this allows this previewObject to be able to snap to multiple snap points



    private void Start()
    {
        buildSystem = GameObject.FindObjectOfType<BuildSystem>();
        myRend = GetComponent<MeshRenderer>();
        ChangeColor();
    }

    public void Place()
    {
        Instantiate(prefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void ChangeColor()
    {
        if (isSnapped)
        {
            myRend.material = goodMat;
        }
        else
        {
            myRend.material = badMat;
        }
    }

    private void Update()
    {
        if(!colWithObjects && inTheGround)
        {
            isSnapped = true;
            ChangeColor();
        }

        if (colWithObjects && !inTheGround)
        {
            isSnapped = false;
            ChangeColor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            inTheGround = true;

        }

        else
        {
            colWithObjects = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            inTheGround = false;
        }

        else
        {
            colWithObjects = false;
        }

    }      


    public bool GetSnapped()
    {
        return isSnapped;
    }




}
