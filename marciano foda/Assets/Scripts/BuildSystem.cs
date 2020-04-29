using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour {


    public Camera cam;//camera used for raycast
    public LayerMask layer;//the layer that the raycast will hit on

    private GameObject previewGameObject = null;//referance to the preview gameobject
    private Preview previewScript = null;//the Preview.cs script sitting on the previewGameObject

    public float stickTolerance = 1.5f;//used for measuring deviation in the mouse when the buildSystem is paused

    [HideInInspector] //hiding this in inspector, so it doesnt accidently get clicked
    public bool isBuilding = false;//are we or are we not currently trying to build something? 
    private bool pauseBuilding = false;//used to pause the raycast



    private void Update()
    {
       // if (Input.GetKeyDown(KeyCode.R))//rotate
      //  {
         //   previewGameObject.transform.Rotate(0, 90f, 0);//rotate the preview 90 degrees. You can add in your own value here
       // }

       // if (Input.GetKeyDown(KeyCode.G))//cancel build
      //  {
        //    CancelBuild();
       // }

        if (Input.GetMouseButtonDown(0) && isBuilding)//actually build the thing in the world
        {
            if (previewScript.GetSnapped())//is the previewGameObject currently snapped to anything? 
            {
                StopBuild();//if so then stop the build and actually build it in the world
            }
            else
            {
                Debug.Log("Not Snapped");//this part isn't needed, but may be good to give your player some feedback if they can't build
            }
        }

        if (isBuilding)
        {
            if (pauseBuilding)//is the build system currently paused? if so then you need to check deviation in the mouse 
            {
                float mouseX = Input.GetAxis("Mouse X");//get the mouses horizontal movement..these may be different on your copy of unity
                float mouseY = Input.GetAxis("Mouse Y");//get the mouses vertical movement..these may be different on your copy of unity

                if (Mathf.Abs(mouseX) >= stickTolerance || Mathf.Abs(mouseY) >= stickTolerance)//check if mouseX or mouseY is greater than stickTolerance
                {
                    pauseBuilding = false;//if it is, then unpause building, and call the raycast again
                }

            }
            else//if building system isn't paused then call the raycast
            {
                DoBuildRay();
            }
        }
    }


    /// <summary>
    /// However you want to start building with the system. This is the method you would need to call
    /// either from your Inventory, HotBar, or some other source.
    /// You will need to pass in a referance to the previewGameObject that you want to build
    /// </summary>

    public void NewBuild(GameObject _go)
    {
        previewGameObject = Instantiate(_go, new Vector3((Screen.width / 2) - (100 / 2), (Screen.height / 2) - (100 / 2), 0), Quaternion.identity);
        previewScript = previewGameObject.GetComponent<Preview>();
        isBuilding = true;
    }

    private void CancelBuild()//you no longer want to build, this will get rid of the previewGameObject in the scene
    {
        Destroy(previewGameObject);
        previewGameObject = null;
        previewScript = null;
        isBuilding = false;
    }

    private void StopBuild()//This is a bad name, It should be called something like BuildIt(). This will actually build your preview in the world
    {
        previewScript.Place();
        previewGameObject = null;
        previewScript = null;
        isBuilding = false; 
    }

    public void PauseBuild(bool _value)//public method to change the pauseBuilding bool from another script. Preview.cs calls this 
                                       //method whereever it snaps to a snap point
    {
        pauseBuilding = _value;
    }

    private void DoBuildRay()
    {
        Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        if(Physics.Raycast(rayOrigin, cam.transform.forward, out hit, 100f, layer))//notice the layer
        {
            /*Since I am using unity primitives in this example I have to postion the previewGameobject in a special way, 
             because of how unity positions things in the scene. If you brought something over from blender, and you have the 
             anchor points setup correctly(located on bottom of model). You can use the line commented out below,
             as opposed to the other lines*/
            
            //If your using unity primitives use these 3 lines
            float y = hit.point.y + (previewGameObject.transform.localScale.y / 2f);
            Vector3 pos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            previewGameObject.transform.position = pos;
            
            //if your using something from blender and anchor points are setup correctly use this line
            //previewGameObject.transform.position = hit.point;
        }
    }

}
