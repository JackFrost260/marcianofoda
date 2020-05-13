
// =================================	
// Namespaces.
// =================================

using UnityEngine;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

    namespace Demos
    {

        namespace TheLastParticle
        {

            // =================================	
            // Classes.
            // =================================

            //[ExecuteInEditMode]
            [System.Serializable]

            public class CameraController : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                public struct ClipPlanePoints
                {
                    public Vector3 UpperLeft;
                    public Vector3 UpperRight;
                    public Vector3 LowerLeft;
                    public Vector3 LowerRight;
                }

                // =================================	
                // Variables.
                // =================================

                // ...

                new Camera camera;

                // Transforms.

                [Header("Transforms")]

                public Transform target;
                public Transform offset;

                // Position.

                [Header("Position")]

                public float followTargetSpeed = 8.0f;

                // Rotation.

                [Header("Rotation")]

                Vector2 mouseInput;

                public float mouseRotateAmount = 5.0f;
                public float mouseRotateSpeed = 5.0f;

                public float rotateZSpeed = 1.0f;
                public float resetZRotationSpeed = 5.0f;

                public Vector2 lookUpDownRange = new Vector2(-80.0f, 80.0f);

                // Zoom.

                [Header("Zoom")]

                public float zoomDistance = 4.0f;

                public float mouseZoomAmount = 1.0f;
                public float mouseZoomSpeed = 5.0f;

                public float mouseZoomMin = 1.0f;
                public float mouseZoomMax = 5.0f;

                // ...

                [Header("Occlusion Zoom")]

                public float occlusionZoomSpeed = 8.0f;
                public float occlusionZoomOffset = 0.0f;

                public float occlusionRaycastDownDistance = 0.5f;

                public AnimationCurve occlisionRaycastDownCurve =
                    new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));

                // Offset framing.

                [Header("Offset (Framing)")]

                public float offsetFramingAmount = 2.0f;
                public float offsetFramingSpeed = 1.0f;

                public float minPlayerSpeedForOffsetFraming = 1.0f;
                public float minMouseMoveForOffsetFraming = 1.0f;

                Vector3 offsetStartPosition;

                Vector3 offsetPositionTarget;
                Vector3 offsetMouseFramePosition;

                // Stored locally as euler because Quaternions take shortest
                // path around for rotation. By using euler, I don't run into 
                // the problem of spinning the camera too quickly and causing 
                // a sudden snap because the rotation exceeded 180 degrees.

                Vector3 rotationEuler;
                Vector3 targetRotationEuler;

                // Player.

                Player player;

                // =================================	
                // Functions.
                // =================================

                // ...

                void Awake()
                {

                }

                // ...

                void Start()
                {
                    lockMouse();

                    camera = Camera.main;
                    player = FindObjectOfType<Player>();

                    zoomDistance = Vector3.Distance(camera.transform.position, target.position);

                    offsetStartPosition = offset.localPosition;
                    offsetPositionTarget = offsetStartPosition;
                }

                // ...

                void lockMouse()
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                void unlockMouse()
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

                // ...

                void FixedUpdate()
                {

                }

                // ...

                void Update()
                {
                    // Input.

                    mouseInput.x = Input.GetAxis("Mouse X");
                    mouseInput.y = Input.GetAxis("Mouse Y");

                    if (Input.GetMouseButtonDown(0))
                    {
                        lockMouse();
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        unlockMouse();
                    }
                }

                // ...

                void LateUpdate()
                {                    
                    // Follow position.

                    transform.position = Damping.dampVector3(transform.position, target.position, followTargetSpeed, Time.deltaTime);

                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        // Rotation.
                        // Can't get it to smooth when rotating, so I just set the physica time step to 0.01f (half of default).

                        targetRotationEuler.y += mouseInput.x * mouseRotateAmount;
                        targetRotationEuler.x += -mouseInput.y * mouseRotateAmount;

                        targetRotationEuler.x = Mathf.Clamp(targetRotationEuler.x, lookUpDownRange.x, lookUpDownRange.y);

                        targetRotationEuler.z += mouseInput.x * rotateZSpeed;

                        rotationEuler = Damping.dampVector3(rotationEuler, targetRotationEuler, mouseRotateSpeed, Time.deltaTime);

                        transform.eulerAngles = rotationEuler;

                        //transform.rotation *= Quaternion.AngleAxis(mouseRotateSpeed * Time.deltaTime, Vector3.up);

                        // Offset framing.

                        float playerSpeed = player.rb.velocity.magnitude;

                        if (playerSpeed < minPlayerSpeedForOffsetFraming)
                        {
                            offsetMouseFramePosition = offsetStartPosition;
                        }
                        else
                        {
                            if (mouseInput.x > minMouseMoveForOffsetFraming)
                            {
                                offsetMouseFramePosition.x = -offsetFramingAmount;
                            }
                            else if (mouseInput.x < -minMouseMoveForOffsetFraming)
                            {
                                offsetMouseFramePosition.x = offsetFramingAmount;
                            }

                            if (mouseInput.y > minMouseMoveForOffsetFraming)
                            {
                                offsetMouseFramePosition.y = -offsetFramingAmount;
                            }
                            else if (mouseInput.y < -minMouseMoveForOffsetFraming)
                            {
                                offsetMouseFramePosition.y = offsetFramingAmount;
                            }
                        }

                        offsetPositionTarget = Damping.dampVector3(offsetPositionTarget, offsetMouseFramePosition, offsetFramingSpeed, Time.deltaTime);

                        // Balance out rotation around Z back to 0.0f.

                        targetRotationEuler.z = Damping.dampFloat(targetRotationEuler.z, 0.0f, resetZRotationSpeed, Time.deltaTime);

                        // Zoom.

                        // Scrolling happens as 0.1f increments. x10.0f to make it 1.0f.

                        float scroll = Input.GetAxis("Mouse ScrollWheel") * 10.0f;

                        zoomDistance -= scroll * mouseZoomAmount;
                        zoomDistance = Mathf.Clamp(zoomDistance, mouseZoomMin, mouseZoomMax);

                        // Player occlusion check and zoom.

                        RaycastHit hitInfo;

                        checkIfPlayerOccluded(out hitInfo);
                        //float nearestDistance = checkCameraPoints(out hitInfo);

                        Vector3 finalOffsetPositionTarget;

                        RaycastHit hitInfo2;

                        Transform cameraTransform = camera.transform;

                        Vector3 directionToTarget = (target.position - cameraTransform.position).normalized;
                        Vector3 end = target.position - (directionToTarget * (zoomDistance + occlusionZoomOffset));

                        Physics.Raycast(end, Vector3.down, out hitInfo2, occlusionRaycastDownDistance);
                        Debug.DrawLine(end, end + (Vector3.down * occlusionRaycastDownDistance), Color.white);

                        if (hitInfo.collider)
                        {
                            //finalOffsetPositionTarget = offsetPositionTarget.normalized * nearestDistance;
                            finalOffsetPositionTarget = offsetPositionTarget.normalized * hitInfo.distance;
                        }
                        //if (hitInfo2.collider)
                        //{                            
                        //    float curveResult = 1.0f - occlisionRaycastDownCurve.Evaluate(hitInfo2.distance / occlusionRaycastDownDistance);
                        //    finalOffsetPositionTarget = (offsetPositionTarget.normalized * zoomDistance) * curveResult;
                        //}
                        //else if (hitInfo.collider)
                        //{
                        //    finalOffsetPositionTarget = Vector3.zero;
                        //}
                        else
                        {
                            finalOffsetPositionTarget = offsetPositionTarget.normalized * zoomDistance;
                        }

                        offset.localPosition = Damping.dampVector3(offset.localPosition, finalOffsetPositionTarget, mouseZoomSpeed, Time.deltaTime);
                    }
                }

                // ...

                void checkIfPlayerOccluded(out RaycastHit hitInfo)
                {
                    if (!camera)
                    {
                        camera = Camera.main;
                    }

                    Transform cameraTransform = camera.transform;

                    Vector3 directionToTarget = (target.position - cameraTransform.position).normalized;

                    Vector3 start = target.position;
                    Vector3 end = Application.isPlaying ?

                        (target.position - (directionToTarget * (zoomDistance + occlusionZoomOffset))) :
                        (offset.position + (-directionToTarget * occlusionZoomOffset));

                    Physics.Linecast(start, end, out hitInfo);

                    if (!Application.isPlaying)
                    {
                        Gizmos.color =
                            hitInfo.collider ? Color.red : Color.green;

                        Gizmos.DrawLine(start, end);
                    }
                    else
                    {
                        Debug.DrawLine(start, end, hitInfo.collider ? Color.red : Color.green);
                    }
                }

                // ...

                void OnDrawGizmos()
                {
                    RaycastHit hitInfo;

                    checkIfPlayerOccluded(out hitInfo);
                    //checkCameraPoints(out hitInfo);
                }

                // =================================	
                // End functions.
                // =================================

            }

            // =================================	
            // End namespace.
            // =================================

        }

    }

}

// =================================	
// --END-- //
// =================================
