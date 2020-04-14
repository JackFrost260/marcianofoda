using UnityEngine;
using System.Collections;

public class ExampleObject : MonoBehaviour
{
    public float rotateSpeed = 15f;

    public Material[] materials = new Material[0];

    private bool isRotating = false;

    private Material currentMaterial = null;

    void Awake()
    {
        if (materials.Length == 0)
        {
            Debug.Log("ExampleObject has no materials set");
        }
        else
        {
            currentMaterial = materials[0];
        }

        isRotating = Random.Range(0, 2) == 0 ? false : true;
    }

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
        }
    }

    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    public void SetRotatingSpeed(float speed)
    {
        isRotating = true;
        rotateSpeed = speed;
    }

    public void SetMaterial(string name)
    {
        string validNames = "";
        for (int i = 0; i < materials.Length; i++)
        {
            if(materials[i].name == name)
            {
                currentMaterial = materials[i];
                if (GetComponent<Renderer>()) GetComponent<Renderer>().material = materials[i];
                return;
            }

            validNames += materials[i].name;
            if (i + 1 != materials.Length) validNames += ", ";
        }

        Debug.Log("Could not find a material named " + name + " in the materials list of the Example Object " + gameObject.name + ". Valid names: " + validNames);
    }

    public string GetCurrentMaterial()
    {
        return currentMaterial.name;
    }
}
